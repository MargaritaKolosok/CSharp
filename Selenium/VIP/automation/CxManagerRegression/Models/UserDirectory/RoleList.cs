using System.Collections.Generic;

namespace Models.UserDirectory
{
    public class RoleListRequest
    {
        public string Name { get; set; }
        public Status Status { get; set; }
        public ApplicationKeyValue Application { get; set; }
        public long? AuthorUserId { get; set; }
        public Pager Pager { get; set; }
    }

    public class RoleListResponse
    {    
        public Pager Pager { get; set; }
        public List<RoleRow> Rows { get; set; }
    }
}
