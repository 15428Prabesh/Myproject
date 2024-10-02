using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ErrorLog: BaseModel
    {
        public DateTime Timestamp { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
    }
}
