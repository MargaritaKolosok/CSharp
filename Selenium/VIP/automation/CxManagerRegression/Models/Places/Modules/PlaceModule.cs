using Newtonsoft.Json;

namespace Models.Places.Modules
{
    public class PlaceModule
    {
        public long ModuleId { get; set; }
        public string Configuration { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ModuleVersion ModuleVersion { get; set; }
    }
}
