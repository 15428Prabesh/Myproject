using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ResponseModel<T>
    {
     //   public bool Status { get; set; } = true;    
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public T Data { get; set; }
       // public List<string> Message { get; set; } = new List<string>();
    }
}
