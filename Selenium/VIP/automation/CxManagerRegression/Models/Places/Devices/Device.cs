using System.Collections.Generic;
using Newtonsoft.Json;

namespace Models.Places.Devices
{
    public class Device
    {
        public long? Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Created { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Updated { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StatusUpdated { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Status { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> DirectItems { get; set; }
        public int? DeviceTypeId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Data Data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AttachmentPlaceTitle { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JsonData { get; set; }
    }
}
