using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Student
    {
        [MaxLength(256)]
        [Required]
        public required string FirstName { get; set; }
        [MaxLength(256)]
        [Required]
        public required string MiddleName { get; set; }
        [MaxLength(256)]
        [Required]
        public required string LastName { get; set; }
        [MaxLength(256)]
        public required string Gender { get; set; }
        [MaxLength(256)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [MaxLength(15)]
        [Required]
        public string Phone { get; set; }
        [Required]
        public required DateTime DOB { get; set; }
        [MaxLength(256)]
        [Required]
        public required string RollNo { get; set; }
        [MaxLength(256)]
        [Required]
        public required string RegistrationNumber { get; set; }
        [MaxLength(256)]
        [Required]
        public required string Course { get; set; }
        [Required]
        public DateTime EnrolledDate { get; set; }
        [Required]
        public required string SemesterID { get; set; }
        public string? DepartmentID { get; set; }
        [Required]
        [MaxLength(256)]
        public required string FatherName { get; set; }
        [MaxLength(15)]
        public required string FatherMobile { get; set; }
        [Required]
        [MaxLength(256)]
        public required string MotherName { get; set; }
        [MaxLength(15)]
        public required string MotherMobile { get; set; }
        [MaxLength(1000)]
        [Required]
        public required string Address { get; set; }
        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public required string ParentEmail { get; set; }
        public FileDocumentDto? Profile { get; set; }
        public List<FileDocumentDto>? Documents { get; set; }

    }
    public class StudentDTO : BaseDTO
    {
        public string Pid { get; set; }
        public string RollNo { get; set; }
        public string RegistrationNumber { get; set; }
        public string Program { get; set; }
        public string Course { get; set; }
        public DateTime EnrolledDate { get; set; }
        public string SemesterID { get; set; }
        public string Semester { get; set; }
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public string FatherName { get; set; }
        public string FatherMobile { get; set; }
        public string MotherName { get; set; }
        public string MotherMobile { get; set; }
        public string Address { get; set; }
        public string ParentEmail { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public bool IsUserCreated { get; set; }
        [JsonIgnore]
        public string UserName { get; set; }
        [JsonIgnore]
        public string ParentUserName { get; set; }
        [JsonIgnore]
        public bool IsParentUserCreated { get; set; }
        public FileDocumentDto? Profile { get; set; }
        public List<FileDocumentDto>? Documents { get; set; }
        public string? CreatedBy { get; set; }
        [JsonIgnore]
        public string? FileDocuments { get; set; }
        [JsonIgnore]
        public string? ProfileImage { get; set; }
    }

    public class StudentListDTO
    {
        [JsonIgnore]
        public int RowTotal { get; set; }
        public string Pid { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string RollNo { get; set; }
        public string RegistrationNumber { get; set; }
        public string Program { get; set; }
        public string Course { get; set; }
        public string Semester { get; set; }
        public bool IsActive { get; set; }
    }
}
