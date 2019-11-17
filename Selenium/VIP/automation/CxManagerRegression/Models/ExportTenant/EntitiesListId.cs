using System.Collections.Generic;

namespace Models.ExportTenant
{
    public class EntitiesListId
    {
        public List<int> Places { get; set; }
        public List<int> Apps { get; set; }
        public List<int> Items { get; set; }
        public string SourceId { get; set; }
    }
}
