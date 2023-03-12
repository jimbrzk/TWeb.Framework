using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWeb.Framework.Abstractions
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public int? UserId { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public string? ClientIp { get; set; }
        public string? BrowserUserAgent { get; set; }
        public AuditLogCategoryEnum Category { get ; set; }
        public string? OtherCategoryName { get; set; }
        public AutitLogLevelEnum Level { get; set; }
    }
}
