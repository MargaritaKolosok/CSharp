namespace Models.Approvals
{
    public class ApprovalRequest
    {
        public long? EntityId { get; set; }
        public int[] Tenants { get; set; }
        public string Comments { get; set; }
        public int EntityType { get; set; }
    }
}
