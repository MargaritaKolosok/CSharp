using Models.Places.Features;

namespace Models.Places.Modules
{
    public class ModuleVersion
    {
        public long? Id { get; set; }
        public long? ModuleId { get; set; }
        public string Schema { get; set; }
        public string DefaultConfiguration { get; set; }
        public string[] I18N { get; set; }
        public Feature[] Features { get; set; }
    }
}
