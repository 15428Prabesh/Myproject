

using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application
{
    [Index(nameof(Pid),IsUnique =true)]
    public abstract class BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ReadOnly(true)]
        public required string Pid { get; set; }
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public  string CreatedBy { get; set; }
        public string? DeletedBy { get; set; }
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
        [DefaultValue(false)]
        public bool IsModified { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

    }
}
