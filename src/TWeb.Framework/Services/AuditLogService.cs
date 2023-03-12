using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TWeb.Framework.Abstractions;
using TWeb.Framework.DAL;

namespace TWeb.Framework.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ILogger<AuditLogService> _logger;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IUserManager _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(ILogger<AuditLogService> logger, IAuditLogRepository auditLogRepository, IUserManager userManager, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _auditLogRepository = auditLogRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<AuditLog> CreateMessageAsync(AuditLogCategoryEnum cat, string message, AutitLogLevelEnum lvl, CancellationToken ct = default)
        {
            string? browserAgent = null;
            string? ip = null;
            User? user = null;

            try { browserAgent = _httpContextAccessor.HttpContext?.Request?.Headers?.FirstOrDefault(x => x.Key == "UserAgent").Value; } catch { }
            try { ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? null; } catch { }
            try { user = await _userManager.GetCurrentUserAsync(ct); } catch { }

            _logger.LogDebug($"{(browserAgent ?? "NO AGENT")}|{(ip ?? "NO IP")}|{(user?.Name ?? "Anonymouse")}| {message}");

            return new AuditLog()
            {
                Date = DateTime.UtcNow,
                BrowserUserAgent = browserAgent,
                ClientIp = ip,
                Message = message,
                UserId = user?.Id,
                Category = cat,
                Level = lvl
            };
        }

        public void Log(AuditLogCategoryEnum cat, string message, AutitLogLevelEnum lvl = AutitLogLevelEnum.Information)
        {
            try
            {
                _auditLogRepository.AddOrUpdate(CreateMessageAsync(cat, message, lvl).Result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to create audit log");
            }
        }

        public async Task LogAsync(AuditLogCategoryEnum cat, string message, AutitLogLevelEnum lvl = AutitLogLevelEnum.Information, CancellationToken ct = default)
        {
            try
            {
                var log = await CreateMessageAsync(cat, message, lvl, ct);
                await _auditLogRepository.AddOrUpdateAsync(log, ct);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to create audit log");
            }
        }
    }
}
