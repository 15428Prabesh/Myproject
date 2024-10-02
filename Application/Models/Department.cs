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
    [Index(nameof(DepartmentName),IsUnique =true)]
    public class Department:BaseModel
    {
        [MaxLength(256)]
        [Required]
        public required string DepartmentName { get; set; }
        [MaxLength(256)]
        [Required]
        public required string Type { get; set; }
        [MaxLength]
        public required string Descriptions { get; set; }
    }

    public class DepartmentDTO
    {
        public int Id { get; set; }
        public required string Pid { get; set; }
        [MaxLength(256)]
        [Required]
        public required string DepartmentName { get; set; }
        [MaxLength(256)]
        [Required]
        public required string Type { get; set; }
        [MaxLength]
        public required string Descriptions { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
        public  string? CreatedBy { get; set; }

    }

    public class CreateDepartmentDTO
    {
        [MaxLength(256)]
        [Required]
        public required string DepartmentName { get; set; }
        [MaxLength(256)]
        [Required]
        public required string Type { get; set; }
        [MaxLength]
        public required string Descriptions { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; }
    }
    public class UpdateDepartmentDTO
    {
        public string DepartmentName { get; set; }
        [MaxLength(256)]
        public string Type { get; set; }
        [MaxLength]
        public string Descriptions { get; set; }
        public bool IsActive { get; set; }
    }





}
