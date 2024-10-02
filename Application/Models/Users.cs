using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    [Index(nameof(UserName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class Users : BaseModel
    {
        [MaxLength(256)]
        public required string UserName { get; set; }
        [MaxLength(256)]
        [Required]
        public required string Email { get; set; }
        public string? Idx { get; set; }
        public int Phone { get; set; }
        public int RoleID { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
    public class UserDTO
    {
        [ReadOnly(true)]
        public string? Pid { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Idx { get; set; }
        public bool IsEmailVerified { get; set; } = true;
        public bool IsPhoneVerified { get; set; } = true;
        public bool IsActive { get; set; } = true;
    }

    public class IdentityUserDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
   
        public string ConfirmPassword { get; set; }

        [Required]
        public List<string> RolesPid { get; set; }
    }

    public class CreateUserDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public List<string> RolesPid { get; set; }

    }

    public class UpdateUserDTO
    {
        [Required]
        public string Idx { get; set; }
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
       [Required]
        public List<string> RolesPid { get; set; }
        [DefaultValue("")]

        public string? Password { get; set; }
        [DefaultValue("")]
        public string? ConfirmPassword { get; set; }


    }

}
