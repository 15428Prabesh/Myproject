namespace SkyLearn.Web.Preview.Models
{
    public class Schema
    {
        public string label { get; set; }
        public string length { get; set; }
        public string api_key { get; set; }
        public Option[] options { get; set; }
        public string help_text { get; set; }
        public string field_type { get; set; }
        public string input_type { get; set; }
        public string validation { get; set; }
        public string multiple_error { get; set; }
        public bool required { get; set; }
        public bool show_label { get; set; }
        public string classname { get; set; }

    }
    public class Option
    {
        public string label { get; set; }
        public string value { get; set; }
    }
}
