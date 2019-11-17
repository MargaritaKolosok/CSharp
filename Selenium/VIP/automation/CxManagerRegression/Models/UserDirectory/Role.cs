using System.Collections.Generic;

namespace Models.UserDirectory
{
    public class Role
    {
        public long? Id { get; set; }
        public ApplicationKeyValue Application { get; set; }
        public string Name { get; set; }
        public Status Status { get; set; }
        public string AuthorUser { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<Group> Groups { get; set; }
        public long? TenantId { get; set; }
    }
}
