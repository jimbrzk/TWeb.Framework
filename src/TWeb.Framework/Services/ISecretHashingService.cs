using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWeb.Framework.Services
{
    public interface ISecretHashingService
    {
        string CreateHash(string secret, string salt);
        bool NeedRehash(string hash);
        bool ValidateHash(string hash, string secret, string salt);
    }
}
