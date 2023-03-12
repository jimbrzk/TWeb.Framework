using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWeb.Framework.Options
{
    public class UserManagerOptions
    {
        public UserManagerCookieOptions Cookie { get; set; }
        public UserManagerJwtOptions Jwt { get; set; }

        public int AccountLockdownAfterFailedAttempts { get; set; }
    }
}
