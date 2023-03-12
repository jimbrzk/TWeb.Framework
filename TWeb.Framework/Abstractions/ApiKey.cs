using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWeb.Framework.Abstractions
{
    public class ApiKey
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int UesrId { get; set; }
        public string Secret { get; set; }
    }
}
