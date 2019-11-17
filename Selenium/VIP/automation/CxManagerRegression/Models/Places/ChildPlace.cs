using Models.Places.Devices;
using Models.Places.Items;

namespace Models.Places
{
    public class ChildPlace
    {
        public Device Device { get; set; }
        public string DeviceName { get; set; }
        public string DeviceTypeName { get; set; }
        public long? Id { get; set; }
        public string Item { get; set; }
        public Item[] Items { get; set; }
        public decimal MapX { get; set; }
        public decimal MapY { get; set; }
        public decimal Radius { get; set; }
        public int Status { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Title { get; set; }
    }
}
