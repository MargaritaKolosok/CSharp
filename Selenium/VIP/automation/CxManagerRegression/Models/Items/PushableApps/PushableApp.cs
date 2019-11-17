using Models.Items.ActualAppVersions;
using Models.Items.References.ReferenceInfo;
using Models.Items.Tags;
using Models.Items.Versions;
using Models.Users;

namespace Models.Items.PushableApps
{
    public class PushableApp
    {
        public long AppId { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public ActualAppVersion ActualAppVersion { get; set; }
        public Version[] Versions { get; set; }
        public ReferenceInfo[] Places { get; set; }
        public Tag[] Tags { get; set; }
        public string ImageName { get; set; }
        public string ThumbnailUrl { get; set; }
        public string FullImageUrl { get; set; }
        public string Updated { get; set; }
        public UserIdName UpdatedBy { get; set; }
        public UserIdName CreatedBy { get; set; }
        public string Created { get; set; }
        public int PushableUsersCount { get; set; }
    }
}
