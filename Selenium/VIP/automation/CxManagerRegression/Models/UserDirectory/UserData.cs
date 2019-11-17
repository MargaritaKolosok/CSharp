using System.Collections.Generic;

namespace Models.UserDirectory
{
    public class UserData
    {
        public long? Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Company { get; set; }
        public Status Status { get; set; }
        public string Created { get; set; }
        public List<Group> Groups { get; set; }
        public List<Role> Roles { get; set; }
        public string TemporaryEmail { get; set; }
        public string Phone { get; set; }
    }
}
