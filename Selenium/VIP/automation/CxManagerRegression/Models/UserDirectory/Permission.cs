namespace Models.UserDirectory
{
    public class Permission
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public object Application { get; set; }
        public Role Role { get; set; }
        public Status Status { get; set; }
    }
}
