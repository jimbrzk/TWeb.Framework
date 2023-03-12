using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWeb.Framework.Attributes
{
    public class TWebAuthorizeAttribute : Attribute
    {
        public bool Anonymouse { get; set; }
    }
}
