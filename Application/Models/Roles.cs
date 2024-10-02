using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    [Index(nameof(RoleName), IsUnique = true)]
    public class Roles : BaseModel
    {
        [MaxLength(256)]
        [Required]
        public required string RoleName { get; set; }
    }
    public class RolesDTO
    {
        [ReadOnly(true)]
        public string? Pid { get; set; }
        [MaxLength(256)]
        [Required]
        public required string RoleName { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = true;
        [MaxLength(256)]
        public string? CreatedBy { get; set; }
    }

   public class CreateRoleDTO
    {
        [Required]
        public string RoleName { get; set; }

        [Required]
        public List<string> PermissionsPid { get; set; }
    }
    public class UpdateRoleDTO 
    {
        [MaxLength(256)]
        [Required]
        public string RoleName { get; set; }
    }
}
