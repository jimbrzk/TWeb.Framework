using BC = BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TWeb.Framework.Services;

namespace TWeb.Framework.BCrypt
{
    public class BCryptHashingService : ISecretHashingService
    {
        public const int WORK_FACTOR = 11;
        public const BC.HashType HASH_TYPE = BC.HashType.SHA384;

        public string CreateHash(string secret, string salt)
        {
            return BC.BCrypt.EnhancedHashPassword(secret + salt, WORK_FACTOR, HASH_TYPE);
        }

        public bool NeedRehash(string hash)
        {
            return BC.BCrypt.PasswordNeedsRehash(hash, WORK_FACTOR);
        }

        public bool ValidateHash(string hash, string secret, string salt)
        {
            return BC.BCrypt.EnhancedVerify(secret + salt, hash, HASH_TYPE);
        }
    }
}
