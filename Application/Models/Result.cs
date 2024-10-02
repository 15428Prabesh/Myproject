using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Result:BaseModel
    {
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        [MaxLength(256)]
        [Required]
        public required string Semester { get; set; }
        [MaxLength(256)]
        [Required]
        public required string ExamName { get; set; }
        public DateTime PublishedDate { get; set; }
        [MaxLength(256)]
        public required string Subject { get; set; }
        [MaxLength(256)]
        public required string Marks { get; set; }
        public int RoleID { get; set; }
    }
    public class ResultDTO
    {
        [ReadOnly(true)]
        public string? Pid { get; set; }
        public string? DepartmentPid { get; set; }
        [MaxLength(256)]
        public required string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = true;
        [MaxLength(256)]
        public string? CreatedBy { get; set; }
    }
    public class CreateResultDTO
    {
        [Required]
        public int DepartmentId { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
