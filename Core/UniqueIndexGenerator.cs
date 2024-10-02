using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class UniqueIndexGenerator
    {
        private static readonly RandomNumberGenerator RandomNumberGenerator = RandomNumberGenerator.Create();

        public static string GenerateUniqueAlphanumericIndex(int? length = 10, string? prefix = "")
        {
            const string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            char[] result = new char[(int)length];

            // Generate a timestamp-based prefix
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string timestampPrefix = Base62Encode(timestamp);

            // Generate a random component
            byte[] randomBytes = new byte[(int)length - timestampPrefix.Length];
            RandomNumberGenerator.GetBytes(randomBytes);
            string randomComponent = new string(randomBytes.Select(b => characters[b % characters.Length]).ToArray());

            // Combine the timestamp prefix and random component
            string uniqueIndex = ((!string.IsNullOrEmpty(prefix)) ? prefix : "") + "_" + timestampPrefix + randomComponent;

            return uniqueIndex;
        }
        private static string Base62Encode(long number)
        {
            const string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            string result = "";

            do
            {
                result = characters[(int)(number % 62)] + result;
                number /= 62;
            } while (number > 0);

            return result;
        }
    }
}
