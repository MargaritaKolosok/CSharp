using System.Collections.Generic;

namespace Models.UserDirectory
{
    public class UserListRequest
    {
        public string Email { get; set; }
        public Pager Pager { get; set; }
    }

    public class UserListResponse
    {
        public Pager Pager { get; set; }
        public List<UserRow> Rows { get; set; }
    }
}
