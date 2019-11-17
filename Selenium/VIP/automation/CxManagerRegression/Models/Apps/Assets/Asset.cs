using Newtonsoft.Json;

namespace Models.Apps.Assets
{
    public class AssetRequest
    {
        public string hash { get; set; }
        public string title { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? minWidth { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? minHeight { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? maxWidth { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? maxHeight { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? minAspectRatio { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? maxAspectRatio { get; set; }
        public string mime { get; set; }
        public string sas { get; set; }
    }

    public class AssetResponse
    {
        public string hash { get; set; }
        public string title { get; set; }
        public int size { get; set; }
        public string jsonData { get; set; }
        public int createdBy { get; set; }
        public string createDate { get; set; }
    }

}
