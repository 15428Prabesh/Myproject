namespace SkyLearn.Web.Preview.Models
{
    public class ComponentModel
    {
        public string Pid { get; set; }
    }

    public class ModelFields
    {
        public required string ApiUrl { get; set; }
        public required string ComponentTitle { get; set; }
        public required string ComponentName { get; set; }
        public string Body { get; set; } = null;
        public required HttpMethod MethodType { get; set; } = HttpMethod.Get;
        public Dictionary<string,string> Header { get; set; }= new Dictionary<string,string>();


    }
}
