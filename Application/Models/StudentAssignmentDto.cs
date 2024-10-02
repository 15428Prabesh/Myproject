using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Models
{
    public class StudentAssignmentDto
    {       

        [JsonIgnore]
        public string RowTotal { get; set; }
        public string StudentAssignmentID { get; set; }
        public string AssignmentName { get; set; }
        public string Semester { get; set; }
        public string DepartmentName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StudentName { get; set; }
        public string StaffName { get; set; }
        public string Status { get; set; }
        public string AssignedOn { get; set; }
    }

    public class StudentAssignmentListDto
    {

        [JsonIgnore]
        public int RowTotal { get; set; }
        public string StudentAssignmentID { get; set; }
        public string AssignmentName { get; set; }
        public string Semester { get; set; }
        public string DepartmentName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StudentName { get; set; }
        public string StaffName { get; set; }
        public string Status { get; set; }
        public string AssignedOn { get; set; }
    }

    public class StudentAssignmentLogListDto
    {

        [JsonIgnore]
        public int RowTotal { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }
        public string CreatedBy { get; set; }       
        public List<FileDocumentDto>? FileDocuments { get; set; }
        [JsonIgnore]
        public string? FileDocument { get; set; }
    }
}
