using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private AuditLog CreateMessage(AuditLogCategoryEnum cat, string message, AutitLogLevelEnum lvl)
        {
            string browserAgent = _httpContextAccessor.HttpContext?.Request?.Headers?.FirstOrDefault(x => x.Key == "UserAgent").Value ?? null;
            string ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? null;
            int? userId = _userManager.GetCurrentUser()?.Id ?? null;

            _logger.LogDebug($"{(browserAgent ?? "NO AGENT")}|{(ip ?? "NO IP")}|{(userId ?? "Anonymouse")}| {message}");

            return new AuditLog()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                BrowserUserAgent = browserAgent,
                ClientIp = ip,
                Message = message,
                UserId = userId,
                Category = cat,
                Level = lvl
            };
        }

        public void Log(AuditLogCategoryEnum cat, string message, AutitLogLevelEnum lvl = AutitLogLevelEnum.Information)
        {
            _auditLogRepository.AddOrUpdate(CreateMessage(cat, message, lvl));
        }

        public Task LogAsync(AuditLogCategoryEnum cat, string message, AutitLogLevelEnum lvl = AutitLogLevelEnum.Information, CancellationToken ct = default)
        {
            return _auditLogRepository.AddOrUpdateAsync(CreateMessage(cat, message, lvl), ct);
        }
    }
}
