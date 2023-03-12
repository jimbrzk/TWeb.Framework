using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWeb.Framework.Abstractions;

namespace TWeb.Framework.Services
{
    public interface IAuditLogService
    {
        void Log(AuditLogCategoryEnum cat, string message, AutitLogLevelEnum lvl = AutitLogLevelEnum.Information, string? otherName = null);
        Task LogAsync(AuditLogCategoryEnum cat, string message, AutitLogLevelEnum lvl = AutitLogLevelEnum.Information, string? otherName = null, CancellationToken ct = default);
    }
}
