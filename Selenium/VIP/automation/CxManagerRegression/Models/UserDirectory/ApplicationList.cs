using System.Collections.Generic;

namespace Models.UserDirectory
{
    public class ApplicationListRequest
    {
        public string Name { get; set; }
        public Pager Pager { get; set; }
    }

    public class ApplicationListResponse
    {
        public Pager Pager { get; set; }
        public List<ApplicationRow> Rows { get; set; }
    }
}
