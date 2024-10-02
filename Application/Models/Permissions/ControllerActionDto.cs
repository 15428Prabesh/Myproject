using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Permissions
{
    public class ControllerActionDto
    {
        public string Pid { get; set; }
        public int Id { get; set; }
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Method { get; set; }
        public bool HasPermission { get; set; }
    }
}
