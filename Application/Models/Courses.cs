using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
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
    public class Courses:BaseModel
    {
        [MaxLength(256)]
        [Required]
        public required string Title { get; set; }
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        [MaxLength]
        [Required]
        public required string Details { get; set; }
        public int Duration { get; set; }
        [MaxLength(256)]
        public required string Features { get; set; }
        [ForeignKey("Semester")]
        public int SemesterId { get; set; }
        public Semester? Semester { get; set; }
        [MaxLength(256)]
        public required string CreditHour { get; set; }
    }
    public class CourseDTO {
        [ReadOnly(true)]
        public string? Pid { get; set; }
        public required string Title { get; set; }
        public required string Details { get; set; }
        public required string Features { get; set; }       
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = true;
        [MaxLength(256)]
        public string? SubmittedBy { get; set; }
    }
    public class CreateCoursesDTO
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public string DepartmentPid { get; set; }
        [Required]
        public string SemesterPid { get; set; }
        [MaxLength]
        [Required]
        public required string Details { get; set; }
        [MaxLength(256)]
        public required string Features { get; set; }
        public bool IsActive { get; set; } = true;
        [Required]
        public string? CreditHour { get; set; }

    }

    public class CourseDetailDTO
    {
        public required string Title { get; set; }
        public required string Details { get; set; }
        public required string Features { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CreditHour { get; set; }

        public Semester semester { get; set; }

        public Department department { get; set; }

    }
    public class UpdateCoursesDTO
    {
        public string SemesterPid { get; set; }
        public string DepartmentPid { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string Features { get; set; }
        public bool IsActive { get; set; }
    }
}
