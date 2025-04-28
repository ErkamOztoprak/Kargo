using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace KargoUygulamasiBackEnd.Helpers
{
    public class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;
        private readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

        public string Hash(string password, out string salt)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
            salt = Convert.ToBase64String(saltBytes);

            using (var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, Algorithm))
            {
                var hash = rfc2898.GetBytes(HashSize);
                return Convert.ToBase64String(hash);    
            }
        }

        public bool Verify(string password, string hash, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using (var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, Iterations, Algorithm))
            {
                var hashBytes = rfc2898.GetBytes(HashSize);
                return Convert.ToBase64String(hashBytes) == hash;
            }
        }
    }

    
}
