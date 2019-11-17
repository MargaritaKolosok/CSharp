using System;
using Newtonsoft.Json;

namespace Models.Items.LangJsonData
{
    public class LangJsonData
    {
        public string en { get; set; }
        [JsonIgnore]
        public LangData.LangData EnJson = new LangData.LangData();
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string de { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string fr { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ar { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string zh { get; set; }
    }
}
