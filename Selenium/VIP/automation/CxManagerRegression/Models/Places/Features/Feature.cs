using Models.Places.Modules;
using Models.Places.Schemas;

namespace Models.Places.Features
{
    public class Feature
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Schema Schema { get; set; }
        public string PlaceTitle { get; set; }
        public ModuleFeatureI18Ns[] ModuleFeatureI18Ns { get; set; }
        public string Status { get; set; }
    }
}
