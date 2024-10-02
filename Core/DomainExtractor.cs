using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class DomainExtractor
    {
        public static string ExtractMainDomain(string url)
        {
            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                string[] hostParts = uri.Host.Split('.');
                if (hostParts.Length >= 2)
                {
                    int startIndex = hostParts.Length >= 3 ? 1 : 0; // Adjust for subdomains
                    string mainDomain = string.Join(".", hostParts, startIndex, hostParts.Length - startIndex);
                    return mainDomain;
                }
                else if(hostParts.Length == 1)
                {
                    return uri.Host;
                }
            }
            return null; // Invalid URL
        }
    }
}
