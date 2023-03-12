using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWeb.Framework.Abstractions;

namespace TWeb.Framework.DAL
{
    public interface IUserTokenRepository
    {
        List<UserToken> List(Func<UserToken, bool> filter, int skip = 0, int limit = 1, Func<UserToken, UserToken>? orderBy = null, bool? orderByDescending = null);

        int Delete(Func<UserToken, bool> filter, int skip, int limit, Func<UserToken, UserToken> orderBy, bool orderByDescending);

        UserToken AddOrUpdate(UserToken user);

        Task<List<UserToken>> ListAsync(Func<UserToken, bool> filter, int skip = 0, int limit = 1, Func<UserToken, UserToken>? orderBy = null, bool? orderByDescending = null, CancellationToken ct = default);

        Task<int> DeleteAsync(Func<UserToken, bool> filter, int skip = 0, int limit = 1, Func<UserToken, UserToken>? orderBy = null, bool orderByDescending = false, CancellationToken ct = default);

        Task<UserToken> AddOrUpdateAsync(UserToken user, CancellationToken ct = default);
    }
}
