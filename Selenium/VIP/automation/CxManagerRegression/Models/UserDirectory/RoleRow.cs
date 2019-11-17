using System.Collections.Generic;

namespace Models.UserDirectory
{
    public class RoleRow
    {
        public long? Key { get; set; }
        public Role Data { get; set; }
        public List<string> Actions { get; set; }
    }
}
