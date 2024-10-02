using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class SubmissionInfo
    {
        public string Remarks { get; set; }
        public List<FileDocumentDto> FileDocuemnt { get; set; }
    }
}
