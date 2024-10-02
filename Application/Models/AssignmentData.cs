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
    public class AssignmentData : BaseModel
    {
        [ForeignKey("Assignment")]
        public int AssignmentId { get; set; }
        public Assignment? Assignment { get; set; }

        public int SubmittedId { get; set; }
        public DateTime SubmittedOn { get; set; }
        public bool ApprovedStatus { get; set; } = false;
    }
    public class AssignmentDataDTO
    {
        [ReadOnly(true)]
        public string? Pid { get; set; }
        [Required]
        [MaxLength(256)]
        public required string Name { get; set; }
        public bool IsActive { get; set; } = false;
    }
    public class CreateAssignmentData
    {
        [MaxLength(256)]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
