using Newtonsoft.Json;

namespace Models.Apps.MasterDataValues
{
    public class MasterDataValues
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string en { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string de { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fr { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zh { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ar { get; set; }
    }
}
