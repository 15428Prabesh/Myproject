using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class StudentAssignmentSummaryDTO
    {
        public int PendingCount { get; set; }
        public int SubmittedCount { get; set; }
        public int RejectedCount { get; set; }
        public int ReSubmittedCount { get; set; }
        public int ApprovedCount { get; set; }
    }
}
