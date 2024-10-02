using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Downloads:BaseModel
    {
        [MaxLength(256)]
        public required string Title { get; set; }
        public string? CoverImage { get; set; }
        public string? Thumbnail { get; set; }
        public string? Extension { get; set; }
        public DateTime ExpiresOn { get; set; }
    }
}
