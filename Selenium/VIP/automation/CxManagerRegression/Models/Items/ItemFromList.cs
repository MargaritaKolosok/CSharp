namespace Models.Items
{
    public class ItemFromList
    {
        public long Id { get; set; }
        public int Status { get; set; }
        public long ModelId { get; set; }
        public bool HasReferences { get; set; }
        public string Picture { get; set; }
    }
}
