using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Staff
    {
        public string DepartmentId { get; set; }
        [MaxLength(256)]
        [Required]
        public string FirstName { get; set; }
        [MaxLength(256)]
        public string MiddleName { get; set; }
        [MaxLength(256)]
        [Required]
        public string LastName { get; set; }
        [MaxLength(256)]
        public string Gender { get; set; }
        [MaxLength(256)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public DateOnly DOB { get; set; }
        [MaxLength(15)]
        [Required]
        public string Phone { get; set; }
        [Required]
        public DateOnly JoiningDate { get; set; }
        [MaxLength(256)]
        [Required]
        public string Position { get; set; }
        [MaxLength(1000)]
        [Required]
        public string Address { get; set; }
        public FileDocumentDto? Profile { get; set; }
        public List<FileDocumentDto>? Documents { get; set; }

    }
    public class StaffDTO : BaseDTO
    {
        public string? Pid { get; set; }
        public int? Id { get; set; }
        public string DepartmentPId { get; set; }
        public string DepartmentName { get; set; }
        public DateOnly JoiningDate { get; set; }
        public string Position { get; set; }
        public string Image { get; set; }
        public string? CreatedBy { get; set; }
        public FileDocumentDto? Profile { get; set; }
        public List<FileDocumentDto>? Documents { get; set; }
        public int RowTotal { get; set; }
        [JsonIgnore]
        public bool IsUserCreated { get; set; }
        [JsonIgnore]
        public string UserName { get; set; }
        [JsonIgnore]
        public string? FileDocuments { get; set; }
        [JsonIgnore]
        public string? ProfileImage { get; set; }
        public bool IsActive { get; set; }
    }
    public class StaffListDTO
    {
        [JsonIgnore]
        public int RowTotal { get; set; }
        public string Pid { get; set; }
        public int? Id { get; set; }
        public string FullName { get; set; }
        public required string Position { get; set; }
        public required string DepartmentName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
    }

}
