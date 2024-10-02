namespace SkyLearn.Web.Preview.Models
{
    public class Content
    {
        public string pid { get; set; }
        public string title { get; set; }

        public string summary { get; set; }

        public string details { get; set; }
        public bool isTitleVisible { get; set; }
        public bool  isActive { get; set; }
    }
}
