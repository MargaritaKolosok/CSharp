namespace Models.ExportTenant
{
    public class EntitiesListInfo
    {
        public Place[] Places { get; set; }
        public App[] Apps { get; set; }
        public Item[] Items { get; set; }
    }

    public class Place
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class App
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Item
    {
        public string Key { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
