using System.Text;
using System.Security.Cryptography;

namespace Identity.Core.Helpers
{
    public static class HashHelper
    {
        public static string GetHash(string password, string solt)
        {
            var passHash = GenerateHash(password);
            var soltHash = GenerateHash(solt);

            return soltHash + passHash;
        }

        private static string GenerateHash(string toHash)
        {
            var crypt = SHA256.Create();

            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(toHash));

            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }

            return hash;
        }
    }
}
