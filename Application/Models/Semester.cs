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
    public class Semester:BaseModel
    {
        [MaxLength(256)]
        public required string Name { get; set; }

        [ForeignKey("Department")]
        public int DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        [NotMapped]
        public virtual string DepartmentPid { get; set; }
    }

    public class SemesterDTO
    {
        [ReadOnly(true)]
        public string? Pid { get; set; }
        [Required]
        [MaxLength(256)]
        public required string Name { get; set; }

        [ReadOnly(true)]
        public DepartmentDTO department { get; set; }

        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
    }

    public class SemesterListDTO
    {
        [ReadOnly(true)]
        public string? Pid { get; set; }
        [Required]
        [MaxLength(256)]
        public required string Name { get; set; }

        public bool IsActive { get; set; } = true;
        public string? CreatedBy { get; set; }
    }

    public class CreateSemesterDTO
    {
        [Required]
        [MaxLength(256)]
        public  string Name { get; set; }
 
        [Required]
        public string DepartmentPid { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateSemesterDTO
    {
        public string DepartmentPid { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }


}
