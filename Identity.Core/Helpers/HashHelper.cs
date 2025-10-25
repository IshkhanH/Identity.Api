using System.Text;
using System.Security.Cryptography;

namespace Identity.Core.Helpers
{
    public static class HashHelper
    {
        public static string GetHash(string password, string salt)
        {
            var passHash = GenerateHash(password);
            var saltHash = GenerateHash(salt);

            return saltHash + passHash;
        }

        private static string GenerateHash(string toHash)
        {

            string hash = String.Empty;
            byte[] crypto = SHA256.HashData(Encoding.ASCII.GetBytes(toHash));

            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }

            return hash;
        }
    }
}
