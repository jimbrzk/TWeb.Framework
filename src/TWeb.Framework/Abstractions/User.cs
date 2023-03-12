using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWeb.Framework.Abstractions
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool Locked { get; set; }
        public int FailedLogons { get; set; }
        public DateTime Created { get; set; }
    }
}
