using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace Application.Models
{
    [Index(nameof(Title), IsUnique = true)]
    public class Content:BaseModel
    {
        [MaxLength(256)]
        [Required]
        public required string Title { get; set; }
        [MaxLength(512)]
        public  string? Summary { get; set; }
        [MaxLength]
        [Required]
        public string? Details { get; set; }
        public bool IsTitleVisible { get; set; } = true;
        public bool IsSummaryVisible { get; set; } = false;
    }
    public class ContentModel
    {
        public string Pid { get; set; }
    }

    public class ContentDTO
    {
        [ReadOnly(true)]
        public string? Pid { get; set; }
        //[Required]
        public required string Title { get; set; }
        public string? Summary { get; set; }
        //[Required]
        public required string? Details { get; set; }
        [DefaultValue(true)]
        public bool IsTitleVisible { get; set; } = true;
        [DefaultValue(false)]
        public bool IsSummaryVisible { get; set; } = false;
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
        //[Required]
        public string CreatedBy { get; set; }
    }

    public class CreateContentDTO
    {
        [Required]
        public required string Title { get; set; }
        public string? Summary { get; set; }
        [Required]
        public required string? Details { get; set; }
        [DefaultValue(true)]
        public bool IsTitleVisible { get; set; } = true;
        [DefaultValue(false)]
        public bool IsSummaryVisible { get; set; } = false;
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }
}
