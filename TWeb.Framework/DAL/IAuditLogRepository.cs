using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWeb.Framework.Abstractions;

namespace TWeb.Framework.DAL
{
    public interface IAuditLogRepository
    {
        List<AuditLog> List(Func<AuditLog, bool> filter, int skip = 0, int limit = 1, Func<AuditLog, AuditLog>? orderBy = null, bool? orderByDescending = null);

        int Delete(Func<AuditLog, bool> filter, int skip, int limit, Func<AuditLog, AuditLog> orderBy, bool orderByDescending);

        AuditLog AddOrUpdate(AuditLog user);

        Task<List<AuditLog>> ListAsync(Func<AuditLog, bool> filter, int skip = 0, int limit = 1, Func<AuditLog, AuditLog>? orderBy = null, bool? orderByDescending = null, CancellationToken ct = default);

        Task<int> DeleteAsync(Func<AuditLog, bool> filter, int skip = 0, int limit = 1, Func<AuditLog, AuditLog>? orderBy = null, bool orderByDescending = false, CancellationToken ct = default);

        Task<AuditLog> AddOrUpdateAsync(AuditLog user, CancellationToken ct = default);
    }
}
