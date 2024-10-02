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
    public class FormData : BaseModel
    {
        [ForeignKey("Form")]
        public int FormID { get; set; }
        public Form? Form { get; set; }
        [MaxLength]
        [Required]
        public required string FormsData { get; set; }
    }
    public class FormDataDTO
    {
        [ReadOnly(true)]
        public string? Pid { get; set; }
        public string? FormsData { get; set; }
        public bool IsActive { get; set; }

    }
    public class CreateFormDataDTO
    {
        [Required]
        public string FormPid { get; set; }
        [Required]
        public string Data { get; set; }
        [Required]
        public string CreatedBy { get; set; }
    }
}
