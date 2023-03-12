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
        void Log(AuditLogCategoryEnum cat, string message, AutitLogLevelEnum lvl = AutitLogLevelEnum.Information);
        Task LogAsync(AuditLogCategoryEnum cat, string message, AutitLogLevelEnum lvl = AutitLogLevelEnum.Information, CancellationToken ct = default);
    }
}
