using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Constants
{
    public class BaseUrl
    {
        public  string PortalBaseUrl { get; set; }
        public string IdentityBaseUrl { get; set; }
    }

    public class FTP
    {
        public string Server { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DocumentHostPath { get; set; }
    }
}
