using System.Collections.Generic;
using Models.Places.AppVersions;
using Models.Places.ParametersInstances;
using Newtonsoft.Json;

namespace Models.Places.Schedules
{
    public class ScheduleApp
    {
        public long? AppId { get; set; }
        public List<AppVersion> AppVersions { get; set; }
        public string AppVersionSelector { get; set; }
        public bool DoDelete { get; set; }
        public long? Id { get; set; }
        public string PackageKey { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ParametersInstance { get; set; }
        [JsonIgnore]
        public ParametersInstance ParametersInstanceJson { get; set; }
        public string Title { get; set; }
        public string _Title { get; set; }
    }
}
