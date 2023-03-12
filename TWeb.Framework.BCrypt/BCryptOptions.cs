using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWeb.Framework.BCrypt
{
    public class BCryptOptions
    {
        public int WorkFactor { get; set; }
        public HashType HashType { get; set; }
    }
}
