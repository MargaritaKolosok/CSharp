namespace Models.Places.AppVersions
{
    public class AppVersion
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long ResourceId { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Revision { get; set; }
        public string ParametersSchema { get; set; }
        public string[] I18N { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Status { get; set; }
        public int[] DeviceTypes { get; set; }
    }
}
