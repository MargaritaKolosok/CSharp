using System.Collections.Generic;
using Models.Interfaces;
using Models.Places.Devices;
using Models.Places.Geolocations;
using Models.Places.Items;
using Models.Places.Modules;
using Models.Places.Schedules;
using Models.Users;
using Newtonsoft.Json;

namespace Models.Places
{
    public class Place : IEntity
    {
        public long? Id { get; set; }
        public bool AppsModuleExists { get; set; }
        public string Title { get; set; }
        public Geolocation Geolocation { get; set; }
        public decimal Radius { get; set; }
        public string Updated { get; set; }
        public string Created { get; set; }
        public string Since { get; set; }
        public string StatusUpdated { get; set; }
        public Device Device { get; set; }
        public List<PlaceModule> PlaceModules { get; set; }
        public string ImageName { get; set; }
        public string ThumbnailUrl { get; set; }
        public string FullImageUrl { get; set; }
        public long TimeZoneId { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string Description { get; set; }
        public UserIdName CreatedBy { get; set; }
        public UserIdName UpdatedBy { get; set; }
        public int? Level { get; set; }
        public bool LiveImages { get; set; }
        public Schedule Schedule { get; set; }
        public List<ScheduleDay> ScheduleDays { get; set; }
        public List<ChildPlace> ChildPlaces { get; set; }
        public decimal MapX { get; set; }
        public decimal MapY { get; set; }
        public decimal MapWidth { get; set; }
        public string MapAssetHash { get; set; }
        public int? DeviceTypeId { get; set; }
        public long? ParentId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? TenantId { get; set; }
        public List<DirectItem> DirectItems { get; set; }
        public List<ParentPlace> Parents { get; set; }
    }

}