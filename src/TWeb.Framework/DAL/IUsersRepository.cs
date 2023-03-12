using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWeb.Framework.Abstractions;

namespace TWeb.Framework.DAL
{
    public interface IUsersRepository
    {
        List<User> List(Func<User, bool> filter, int skip = 0, int limit = 1, Func<User, User>? orderBy = null, bool? orderByDescending = null);

        int Delete(Func<User, bool> filter, int skip, int limit, Func<User, User> orderBy, bool orderByDescending);

        User AddOrUpdate(User user);

        Task<List<User>> ListAsync(Func<User, bool> filter, int skip = 0, int limit = 1, Func<User, User>? orderBy = null, bool? orderByDescending = null, CancellationToken ct = default);

        Task<int> DeleteAsync(Func<User, bool> filter, int skip = 0, int limit = 1, Func<User, User>? orderBy = null, bool orderByDescending = false, CancellationToken ct = default);

        Task<User> AddOrUpdateAsync(User user, CancellationToken ct = default);
    }
}
