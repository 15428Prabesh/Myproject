using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Component:BaseModel
    {
        [MaxLength(256)]
        public required string Name { get; set; }

        [MaxLength]
        public string? Schema { get; set; }
    }

    public class ComponentData:BaseModel
    {
        [ForeignKey("Component")]
        public int ComponentId { get; set; }
        public virtual Component? Component { get; set; }

        [MaxLength]
        public required string Data { get; set; }
    }

    public class ComponentDTO
    {
        public string Pid { get; set; }
        [Required]
        public required string Name { get; set; }

        [MaxLength]
        public string? Schema { get; set; }
        public bool IsActive { get; set; } = true;

    }

    public class ComponentDataDTO
    {
        public string Pid { get; set; }
        public  Component? Component { get; set; }

        [Required]
        public  string? Data { get; set; }
    }

    public class CreateComponentDataDTO
    {
        [Required]
        public string ComponentPid { get; set; }

        [Required]
        public string Data { get; set; }

        [Required]
        public string CreatedBy { get; set; }
    }
}
