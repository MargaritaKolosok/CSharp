namespace Models.UserDirectory
{
    public class ApplicationData
    {
        public string ApplicationId { get; set; }
        public string Created { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public string PermissionsServiceUrl { get; set; }
        public Status Status { get; set; }
        public bool TenantBased { get; set; }
        public string Title { get; set; }  
    }
}
