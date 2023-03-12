using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TWeb.Framework.Abstractions;
using TWeb.Framework.DAL;
using TWeb.Framework.Exceptions;
using TWeb.Framework.Options;
using TWeb.Framework.Services;

namespace TWeb.Framework
{
    public class UserManager : IUserManager
    {
        private readonly ILogger<UserManager> _logger;
        private readonly IEmailSenderService _email;
        private readonly IUsersRepository _usersRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IApiKeysRepository _apiKeyRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISecretHashingService _secretHashingService;
        private readonly IAuditLogService _auditLogService;
        private readonly IOptionsMonitor<UserManagerOptions> _options;
        private readonly JwtSecurityTokenHandler _jwtHandler;
        private TokenValidationParameters _jwtValidationParameters => new TokenValidationParameters()
        {
            RequireAudience = true,
            ValidateAudience = true,
            ValidAudience = _options.CurrentValue.Jwt.Audience,
            ValidateIssuer = true,
            ValidIssuer = _options.CurrentValue.Jwt.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.CurrentValue.Jwt.Key)),
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.Zero
        };

        public UserManager(ILogger<UserManager> logger, IEmailSenderService email, IUsersRepository usersRepository, IHttpContextAccessor httpContextAccessor, ISecretHashingService secretHashingService, IOptionsMonitor<UserManagerOptions> optionsMonitor, IUserTokenRepository userTokenRepository, IApiKeysRepository apiKeysRepository, IAuditLogService auditLogService)
        {
            _logger = logger;
            _email = email;
            _usersRepository = usersRepository;
            _httpContextAccessor = httpContextAccessor;
            _secretHashingService = secretHashingService;
            _options = optionsMonitor;
            _userTokenRepository = userTokenRepository;
            _apiKeyRepository = apiKeysRepository;
            _auditLogService = auditLogService;
            _jwtHandler = new JwtSecurityTokenHandler();
        }

        public IUsersRepository Users => _usersRepository;

        public void ChangePassword(User user, string currentPassword, string newPassword)
        {
            if (user == null) throw new UserNotFoundException();
            user = _usersRepository.List(x => x.Id == user.Id).FirstOrDefault();
            if (user == null) throw new UserNotFoundException();

            if (!_secretHashingService.ValidateHash(user.Password, currentPassword, user.Salt))
                throw new InvalidUserPasswordException();

            SetUserPassword(newPassword, ref user);

            _usersRepository.AddOrUpdate(user);

            _auditLogService?.Log(AuditLogCategoryEnum.User, "Password has changed");

            SendEmail(user, EmailOperation.PasswordChanged);
        }

        public User Login(string email, string password)
        {
            var user = _usersRepository.List(x => x.Email.ToLower() == email.ToLower()).FirstOrDefault();
            if (user == null) throw new UserNotFoundException();

            CheckUserIsLocked(user);

            if (!_secretHashingService.ValidateHash(user.Password, password, user.Salt))
            {
                if(_options.CurrentValue.AccountLockdownAfterFailedAttempts > 0)
                {
                    user.FailedLogons++;
                    if(_options.CurrentValue.AccountLockdownAfterFailedAttempts >= user.FailedLogons)
                    {
                        user.Locked = true;
                        _auditLogService?.Log(AuditLogCategoryEnum.User, $"Account of user '{user.Name}' has bean locked becaouse of too many invalid logon attempts.", AutitLogLevelEnum.Alert);
                        SendEmail(user, EmailOperation.AccountLocked, _httpContextAccessor.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "UserAgent").Value, _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString());
                    }
                    _usersRepository.AddOrUpdate(user);
                    throw new UserLockedException();
                }
                throw new InvalidUserPasswordException();
            }
            if (_secretHashingService.NeedRehash(user.Password))
            {
                SetUserPassword(password, ref user);
                _usersRepository.AddOrUpdate(user);
            }

            var jwt = GenerateToken(user);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(_options.CurrentValue.Cookie.Name, _jwtHandler.WriteToken(jwt));
            _httpContextAccessor.HttpContext.Items["User"] = user;

            _auditLogService?.Log(AuditLogCategoryEnum.User, "User login");

            SendEmail(user, EmailOperation.Logon, _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(), DateTimeOffset.UtcNow.ToString(), _httpContextAccessor.HttpContext.Request.Headers.FirstOrDefault(x => x.Key == "UserAgent").Value);

            return user;
        }

        public bool Logout()
        {
            _auditLogService?.Log("User logout");

            _httpContextAccessor.HttpContext.Response.Cookies.Delete(_options.CurrentValue.Cookie.Name, new CookieOptions() { Expires = DateTimeOffset.UtcNow });
            _httpContextAccessor.HttpContext.Items.Remove("User");
            return true;
        }

        public User Profile()
        {
            var user = GetCurrentUser();
            if (user == null) throw new UserNotFoundException();
            return user;
        }

        public void ResetPassword(string email)
        {
            if (_email == null) throw new Exception($"Password reset is not posible. No {nameof(IEmailSenderService)} implementation was loaded for dependency injection.");

            var user = _usersRepository.List(x => x.Email == email).FirstOrDefault();
            if (user == null) return;

            string newPass = GeneratePassword();
            SetUserPassword(newPass, ref user);

            _usersRepository.AddOrUpdate(user);

            _auditLogService?.Log(AuditLogCategoryEnum.User, $"User '{user.Name}' password reset");

            SendEmail(user, EmailOperation.PasswordReset, newPass);
        }

        private void SetUserPassword(string newPass, ref User user)
        {
            user.Salt = MD5.HashData(Encoding.UTF8.GetBytes($"{user.Id}{$"{user.Id}{user.Email}{user.Email}".GetHashCode()}"));
            user.Password = _secretHashingService.CreateHash(newPass, user.Salt);
        }

        public void CheckUserIsLocked(User user)
        {
            if (user.Locked)
            {
                Logout();
                throw new UserLockedException();
            }
        }

        public User? GetCurrentUser()
        {
            int userId = 0;

            if (!_httpContextAccessor.HttpContext.Items.Any(x => x.Key == "User"))
            {
                var cookie = _httpContextAccessor.HttpContext.Request.Cookies.FirstOrDefault(x => x.Key == _options.CurrentValue.Cookie.Name).Value;
                if (cookie == null) return null;

                var claim = _jwtHandler.ValidateToken(cookie, _jwtValidationParameters, out SecurityToken token);

                if (!int.TryParse(claim.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.NameId)?.Value, out userId)) return null;
            }
            else
            {
                userId = ((User)_httpContextAccessor.HttpContext.Items["User"]).Id;
            }

            var user = _usersRepository.List(x => x.Id == userId)?.FirstOrDefault();

            if (user == null) throw new UserNotFoundException();
            CheckUserIsLocked(user);

            return user;
        }

        private JwtSecurityToken GenerateToken(User user)
        {
            var jwtParams = _jwtValidationParameters;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString())
            };

            var signingCredentials = new SigningCredentials(jwtParams.IssuerSigningKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: jwtParams.ValidIssuer,
                audience: jwtParams.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.Add(_options.CurrentValue.Jwt.Expiration),
                signingCredentials: signingCredentials,
                notBefore: DateTime.UtcNow);
            return jwtSecurityToken;
        }

        private string GeneratePassword(int requiredLen = 8)
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";

            char[] chars = new char[requiredLen];
            for (int i = 0; i < requiredLen; i++)
            {
                chars[i] = validChars[RandomNumberGenerator.GetInt32(0, validChars.Length-1)];
            }

            return new string(chars);
        }

        private void SendEmail(User user, EmailOperation op, params string[] vars = null)
        {
            _logger.LogDebug($"Sending email {op}");

            if (_email == null) return;

            Func<string, string> createBody = new Func<string, string>((body) =>
            {
                string header = "<html><head><style>html { margin: 1px; } body { margin: 9px; color: black; background-color: grey; border: 1; border-radius: 2px; font-family: Tahoma, sans-serif; } pre { background-color: black; color: white; padding: 1px; } h1 { font-size:5vw; } p { font-size:2vw; }</style></head><body>";
                string footer = "</body></html>";
                return $"{header}{Environment.NewLine}{body}{Environment.NewLine}{footer}";
            });

            try
            {
                switch (op)
                {
                    case EmailOperation.PasswordReset:
                        _email.SendEmail(user.Email, "Password reset", createBody.Invoke($"<h1>Hi {user.Name}!</h1><p>Your new password: <pre><b>{vars[0]}</b></pre></p><br><p><i>Change your password ASAP!</i></p>"))
                        break;
                    case EmailOperation.Logon:
                        _email.SendEmail(user.Email, "New logon", createBody.Invoke($"<h1>Hi {user.Name}!</h1><p>We have detected new logon to your account at {vars[1]} from IP {vars[0]}, user agent {vars[2]}</p>"))
                        break;
                    case EmailOperation.PasswordChanged:
                        _email.SendEmail(user.Email, "Password changed", createBody.Invoke($"<h1>Hi {user.Name}!</h1><p>Your password has bean changed.</p>"))
                        break;     
                    case EmailOperation.AccountLocked:
                        _email.SendEmail(user.Email, "Account locked", createBody.Invoke($"<h1>Hi {user.Name}!</h1><p>Your account has bean locked becaouse theres to many invalid logon attemptes.</p><p><b>Last invalid logon</b><br><ul><li>Date: {DateTime.UtcNow}</li><li>User agent: {vars[0]}</li><li>IP: {vars[1]}</li></ul></p>"))
                        break;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email. {ex.Message}");
            }
        }

        private struct EmailOperation
        {
            public const string PasswordReset = "PASSWORD_RESET";
            public const string PasswordChanged = "PASSWORD_CHANGED";
            public const string Logon = "LOGON";
            public const string AccountLocked = "ACCOUNT_LOCKDOWN";
        }
    }
}
