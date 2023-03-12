using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWeb.Framework.Exceptions
{
    public class UserNotAuthenticatedException : Exception
    {
        public UserNotAuthenticatedException() : base() { }
        public UserNotAuthenticatedException(string msg, Exception ex) : base(msg, ex) { }
    }
}
