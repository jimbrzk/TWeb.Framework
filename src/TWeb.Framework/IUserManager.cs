using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWeb.Framework.Abstractions;
using TWeb.Framework.DAL;
using TWeb.Framework.Exceptions;

namespace TWeb.Framework
{
    public interface IUserManager
    {
        void ResetPassword(string email);

        void ChangePassword(User user, string currentPassword, string newPassword);

        User Login(string username, string password);

        bool Logout();

        User Profile();
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="UserNotAuthenticatedException"></exception>
        /// <exception cref="UserNotFoundException"></exception>
        /// <exception cref="UserLockedException"></exception>
        /// <returns></returns>
        User GetCurrentUser();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <exception cref="UserNotAuthenticatedException"></exception>
        /// <exception cref="UserNotFoundException"></exception>
        /// <exception cref="UserLockedException"></exception>
        /// <returns></returns>
        Task<User> GetCurrentUserAsync(CancellationToken ct = default);

        IUsersRepository Users { get; }
    }
}
