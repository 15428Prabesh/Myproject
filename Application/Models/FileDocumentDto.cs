using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class FileDocumentDto
    {
        public string FileName { get; set; }
        public string? FilePath {  get; set; }
        public string? ByteString { get; set; }
    }
}
