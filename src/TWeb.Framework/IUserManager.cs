using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWeb.Framework.Abstractions;
using TWeb.Framework.DAL;

namespace TWeb.Framework
{
    public interface IUserManager
    {
        void ResetPassword(string email);

        void ChangePassword(User user, string currentPassword, string newPassword);

        User Login(string username, string password);

        bool Logout();

        User Profile();

        User? GetCurrentUser();

        IUsersRepository Users { get; }
    }
}
