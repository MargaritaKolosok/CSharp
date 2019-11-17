using System.Collections.Generic;
using Models.Apps.ActualAppVersions;
using Models.Apps.Places;
using Models.Apps.Tags;
using Models.Interfaces;
using Models.Users;
using Newtonsoft.Json;
using Version = Models.Apps.Versions.Version;

namespace Models.Apps
{
    public class AppRequest : IEntity, IApprovalEntity
    {
        public long AppId { get; set; }
        public string Type { get; set; } 
        public string Title { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public ActualAppVersionRequest ActualAppVersion { get; set; }
        public string StatusName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Version> Versions { get; set; }
        public List<Place> Places { get; set; }
        public List<Tag> Tags { get; set; }
        public string ImageName { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ShowImageUrl { get; set; }
        public string ShowFullImageUrl { get; set; }
        public string FullImageUrl { get; set; }
        public string Updated { get; set; }
        public UserIdName UpdatedBy { get; set; }
        public UserIdName CreatedBy { get; set; }
        public string Created { get; set; }
        public int PushableUsersCount { get; set; }
        public string CreatedName { get; set; }
    }
}
