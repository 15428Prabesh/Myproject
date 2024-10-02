using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class UserRole : BaseModel
    {
        public required string UserIdx { get; set; }
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Roles Role
        {
            get; set;
        }
    }
    public class UserRoleDTO
    {
        public int Id { get; set; }
        public string UserIdx { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class CreateUserRoleDTO
    {
        public string UserIdx { get; set; }
        public int RoleId { get; set; }
       // public required string RoleName { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class RoleListDTO{
        public string Name {get;set;}
        public string Pid {get;set;}
    }
}
