using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class AppHelper
    {
        public static string GeneratePid(string prefix)
        {
            if (prefix.Length == 3)
            {
                Guid guid = Guid.NewGuid();
                string guidstr = guid.ToString("N");
                if (guidstr.Length > 12)
                {
                    return prefix + "_" + guidstr.Substring(0, 12);
                }
                else
                {
                    return prefix + "_" + guidstr;
                }
            }
            throw new Exception("prefix cannot be larger or smaller than 3");

        }

        public static int GenerateRandomNumber(int Min, int Max)
        {
            Random random = new Random();
            return random.Next(Min,Max);

        }

        public static string GenerateRandomStrings(int length=6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            char[] randomArray = new char[length];

            for (int i = 0; i < length; i++)
            {
                randomArray[i] = chars[random.Next(chars.Length)];
            }

            return new string(randomArray);
        }

    }

   


}
