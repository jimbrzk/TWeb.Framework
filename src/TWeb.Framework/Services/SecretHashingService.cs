using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TWeb.Framework.Services
{
    [Obsolete]
    public class SecretHashingService : ISecretHashingService
    {
        public string CreateHash(string secret, string salt)
        {
            //Convert the salted password to a byte array
            byte[] saltedHashBytes = Encoding.UTF8.GetBytes(secret + salt);

            //Use hash algorithm to calulate hash
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] hash = algorithm.ComputeHash(saltedHashBytes);

            //Return the hash as a base64 encoded string to be compared and stored
            return Convert.ToBase64String(hash);
        }

        public bool NeedRehash(string hash) => false;

        public bool ValidateHash(string hash, string secret, string salt)
        {
            return CreateHash(secret, salt).Equals(hash);
        }
    }
}
