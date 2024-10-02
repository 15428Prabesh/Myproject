using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class RolePermission
    {
        [ForeignKey("Roles")]
        [Required]
        public int RoleId { get; set; }
        public virtual Roles? Roles { get; set; }
        public int role_id { get; set; }
        [ForeignKey("Permisssion")]
        [Required]
        public int PermissionId { get; set; }
        public virtual ActionPermission? Permission { get; set; }
    }
}
