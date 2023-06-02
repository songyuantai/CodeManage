namespace CodeManager.Models
{
    public class BuildViewModel
    {
        public string ConnectId { get; set; }

        public List<string> TableIds { get; set; }

        public string OutputPath { get; set; }

        public string NameSpace { get; set; }

        public string Template { get; set; }
    }
}
