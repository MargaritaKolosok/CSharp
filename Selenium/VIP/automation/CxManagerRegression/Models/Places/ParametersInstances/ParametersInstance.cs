using Models.Places.Items;
using Newtonsoft.Json;

namespace Models.Places.ParametersInstances
{
    public class ParametersInstance
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JsonItem item { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public JsonItem poi { get; set; }
    }
}
