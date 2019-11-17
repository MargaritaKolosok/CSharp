using Newtonsoft.Json;

namespace Models.Items.References.ReferenceInfo
{
    public class ReferenceInfo
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? DirectAssignments { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Status { get; set; }
    }
}
