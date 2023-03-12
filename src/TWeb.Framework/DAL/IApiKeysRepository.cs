using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWeb.Framework.Abstractions;

namespace TWeb.Framework.DAL
{
    public interface IApiKeysRepository
    {
        List<ApiKey> List(Func<ApiKey, bool> filter, int skip = 0, int limit = 1, Func<ApiKey, ApiKey>? orderBy = null, bool? orderByDescending = null);

        int Delete(Func<ApiKey, bool> filter, int skip, int limit, Func<ApiKey, ApiKey> orderBy, bool orderByDescending);

        ApiKey AddOrUpdate(ApiKey user);

        Task<List<ApiKey>> ListAsync(Func<ApiKey, bool> filter, int skip = 0, int limit = 1, Func<ApiKey, ApiKey>? orderBy = null, bool? orderByDescending = null, CancellationToken ct = default);

        Task<int> DeleteAsync(Func<ApiKey, bool> filter, int skip, int limit, Func<ApiKey, ApiKey> orderBy, bool orderByDescending, CancellationToken ct = default);

        Task<ApiKey> AddOrUpdateAsync(ApiKey user, CancellationToken ct = default);
    }
}
