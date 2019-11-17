using System.Collections.Generic;
using Models.Tenants.CreatedUpdatedBy;

namespace Models.Tenants
{
    public class Tenant
    {
        public string Code { get; set; }
        public List<string> CodeAliases { get; set; }
        public string Created { get; set; }
        public CreatedBy CreatedBy { get; set; }
        public int Id { get; set; }
        public int Level { get; set; }
        public int? ParentId { get; set; }
        public List<string> Permissions { get; set; }
        public string Title { get; set; }
        public string Updated { get; set; }
        public CreatedBy UpdatedBy { get; set; }
        public List<int> DuplicateTenants { get; set; }
    }
}
