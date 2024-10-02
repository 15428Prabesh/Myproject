using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace Application.Models
{
    [Index(nameof(Title), IsUnique = true)]
    public  class Form:BaseModel
    {
        [MaxLength(256)]
        public required string Title { get; set; }
        [MaxLength]
        public required string Schema { get; set; }
    }
    public class FormDTO
    {
        public string? Pid { get; set; }
        public string? Title { get; set; }
        public string? Schema { get; set; }

    }

}
