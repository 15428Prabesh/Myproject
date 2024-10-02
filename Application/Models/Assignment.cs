using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Assignment : BaseModel
    {
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Courses? Course { get; set; }
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        [ForeignKey("Semester")]
        public int SemesterId { get; set; }
        public Semester? Semester { get; set; }
        [Required]
        [MaxLength(256)]
        public required string Name { get; set; }
        [MaxLength(512)]
        public required string AssignmentDetails { get; set; }
        public decimal Marks { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [NotMapped]
        public string? CoursePid { get; set; }
        public int StaffID { get; set; }
        public string? FileDocuments { get; set; }
    //        public ICollection<StudentAssignment> StudentAssignments { get; set; }

}
//public class StudentAssignment:BaseModel
//{


//    public int StudentId { get; set; }
//    [ForeignKey("StudentId")]
//    public Student Student { get; set; }

//    public int AssignmentId { get; set; }
//    [ForeignKey("AssignmentId")]
//    public Assignment? Assignment { get; set; }

//    public int StatusId { get; set; }
//    [ForeignKey("StatusId")]
//    public Status? AssignmentStatus { get; set; }
//}

public class AssignmentDTO
{
    [ReadOnly(true)]
    public string? Pid { get; set; }
    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }
    [MaxLength(512)]
    public required string AssignmentDetails { get; set; }
    public decimal Marks { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    [MaxLength(256)]
    public string? PreparedBy { get; set; }
}
public class CreateAssignmentDTO
{
    [Required]
    public string CoursePid { get; set; }
    [Required]
    public string DepartmentPid { get; set; }
    [Required]
    public string SemesterPid { get; set; }
    public string StaffPid { get; set; }
    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }
    [MaxLength(512)]
    public required string AssignmentDetails { get; set; }
    public decimal Marks { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; } = true;
    [JsonIgnore]
    public string? FileDocuments { get; set; }

}
public class AssignmentDetailDTO
{
    [ReadOnly(true)]
    public string? Pid { get; set; }
    [Required]
    [MaxLength(256)]
    public required string Name { get; set; }
    [MaxLength(512)]
    public required string AssignmentDetails { get; set; }
    public decimal Marks { get; set; }
    public DepartmentsDTO Department { get; set; }
    public CoursesDTO Course { get; set; }
    public SemestersDTO Semester { get; set; }
    //public StaffsDTO Staff { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    [MaxLength(256)]
    public string? PreparedBy { get; set; }

}
public class UpdateAssignmentDTO
{
    public string? CoursePid { get; set; }
    public string? DepartmentPid { get; set; }
    public string? SemesterPid { get; set; }
    public string? StaffPid { get; set; }

    [MaxLength(256)]
    public string Name { get; set; }
    [MaxLength(512)]
    public string AssignmentDetails { get; set; }
    public decimal Marks { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
public class DepartmentsDTO
{
    public string Pid { get; set; }
    public string Name { get; set; }
}
public class CoursesDTO
{
    public string Pid { get; set; }
    public string Name { get; set; }
}
public class SemestersDTO
{
    public string Pid { get; set; }
    public string Name { get; set; }
}
public class StaffsDTO
{
    public string Pid { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
}
