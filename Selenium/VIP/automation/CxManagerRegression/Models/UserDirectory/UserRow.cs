using System.Collections.Generic;

namespace Models.UserDirectory
{
    public class UserRow
    {
        public long? Key { get; set; }
        public UserData Data { get; set; }
        public List<string> Actions { get; set; }
    }
}
