using System.Collections.Generic;

namespace Models.UserDirectory
{
    public class ApplicationRow
    {
        public string Key { get; set; }
        public ApplicationData Data { get; set; }
        public List<string> Actions { get; set; }
    }
}
