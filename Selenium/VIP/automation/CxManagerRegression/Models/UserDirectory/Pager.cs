namespace Models.UserDirectory
{
    public class Pager
    {
        public int StartRecordIndex { get; set; }
        public int RecordsPerPage { get; set; }
        public string SortBy { get; set; }
        public bool SortDesc { get; set; }
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
    }
}
