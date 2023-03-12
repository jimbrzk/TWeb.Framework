using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWeb.Framework.DAL;
using TWeb.Framework.Services;

namespace TWeb.Framework.Extensions
{
    public static class AuditLogExtension
    {
        public static IServiceCollection AddTWebAudit<TAuditLogRepository>(this IServiceCollection services) where TAuditLogRepository : class, IAuditLogRepository => AddTWebAudit<TAuditLogRepository, AuditLogService>(services);

        public static IServiceCollection AddTWebAudit<TAuditLogRepository, TAuditLogService>(this IServiceCollection services) 
            where TAuditLogRepository : class, IAuditLogRepository
            where TAuditLogService : class, IAuditLogService
        {
            return services
                .AddHttpContextAccessor()
                .AddTransient<IAuditLogRepository, TAuditLogRepository>()
                .AddTransient<IAuditLogService, TAuditLogService>();
        }
    }
}
