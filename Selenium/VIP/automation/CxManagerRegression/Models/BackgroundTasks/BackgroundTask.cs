namespace Models.BackgroundTasks
{
    public class BackgroundTask
    {
        public string TaskId { get; set; }
        public long UserId { get; set; }
        public string Created { get; set; }
        public long Id { get; set; }
        public string JsonData { get; set; }
        public int Progress { get; set; }
        public long FileSize { get; set; }
        public string FileSas { get; set; }
        public string FileName { get; set; }
        public string TenantCode { get; set; }
        public int TenantId { get; set; }
        public string TenantTitle { get; set; }
        public int Status { get; set; }
        public int ActionType { get; set; }
    }
}
