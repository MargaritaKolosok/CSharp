namespace Models.UserDirectory
{
    public class Group
    {
        public int? Id { get; set; }
        public object Name { get; set; }
        public object Status { get; set; }
        public object AuthorUser { get; set; }
        public object[] Roles { get; set; }
    }
}
