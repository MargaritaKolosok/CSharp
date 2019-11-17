namespace Models.Places.Devices
{
    public class DeviceChildPlace
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int DeviceTypeId { get; set; }
        public string Updated { get; set; }
        public string ImageName { get; set; }
        public string ThumbnailUrl { get; set; }
        public Place Place { get; set; }
        public string JsonData { get; set; }
    }
}
