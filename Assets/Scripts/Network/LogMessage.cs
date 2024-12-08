using System.Collections.Generic;

namespace Prog2step.Models
{
    public class LogMessage
    {
        public string Comment { get; set; }
        public string Name { get; set; }
        public string Shop_name { get; set; }
        public Dictionary<string, string> ResourcesChanged { get; set; }
    }
}
