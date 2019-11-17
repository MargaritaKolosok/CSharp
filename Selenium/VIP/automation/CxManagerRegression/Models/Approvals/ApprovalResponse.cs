namespace Models.Approvals
{
    public class ApprovalResponse
    {
        public long? EntityId { get; set; }
        public string Action { get; set; }
        public string Comments { get; set; }
        public int EntityType { get; set; }
    }
}
