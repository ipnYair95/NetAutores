using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using WebApp.DTOs;

namespace WebApp.Services
{
    public class HashService
    {
        public ResultadoHash Hash(string texto)
        {
            var sal = new byte[16];

            using ( var random = RandomNumberGenerator.Create() )
            {
                random.GetBytes(sal);
            }

            return Hash(texto, sal);
        }

        public ResultadoHash Hash(string texto, byte[] sal)
        {
            var llaveDerivada = KeyDerivation.Pbkdf2(
                password: texto,
                salt: sal,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 32
                );

            var hash = Convert.ToBase64String(llaveDerivada);

            return new ResultadoHash()
            {
                Hash = hash,
                Sal = sal
            };
        }

    }
}
