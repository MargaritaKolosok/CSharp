using Models.Users;

namespace Models.Items.ActualAppVersions
{
    public class ActualAppVersion
    {
        public string Title { get; set; }
        public long Id { get; set; }
        public UserIdName CreatedBy { get; set; }
        public string Created { get; set; }
        public string Updated { get; set; }
        public UserIdName UpdatedBy { get; set; }
        public string ImageName { get; set; }
        public string ThumbnailUrl { get; set; }
        public string FullImageUrl { get; set; }
        public int Status { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public string MasterData { get; set; }
        public string AppType { get; set; }
        public string ApiKey { get; set; }
        public string AllowedApis { get; set; }
        public string PushableItemTypes { get; set; }
        public string AppUserEmail { get; set; }
        public string[] DeviceTypes { get; set; }
        public MasterDataValues.MasterDataValues MasterDataValues { get; set; }
        public OverwritableItems.OverwritableItems OverwritableItems { get; set; }
        public string[] I18N { get; set; }
    }
}
