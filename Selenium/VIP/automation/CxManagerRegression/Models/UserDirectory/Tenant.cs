using System.Collections.Generic;

namespace Models.UserDirectory
{
    public class Tenant
    {
        public string Code { get; set; }
        public string Created { get; set; }
        public int? CreatedBy { get; set; }
        public int? Level { get; set; }
        public int? ParentId { get; set; }
        public Status Status { get; set; }
        public int TenantId { get; set; }
        public string Title { get; set; }
        public string Updated { get; set; }
        public int? UpdatedBy { get; set; }
        public string CountryCode { get; set; }
        public List<int> Langs { get; set; }
        public List<int> DuplicateTenants { get; set; }
        public bool ApprovalEnabled { get; set; }
    }
}
