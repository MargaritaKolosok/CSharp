using System.Collections.Generic;
using Models.Tenants;

namespace Models.Users
{
    public class AuthorizationRequest
    {
        public bool StayLoggedIn { get; set ; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DateTime { get; set; }
        public string ClientIp { get; set; }
    }

    public class AuthorizationResponse
    {
        public int UserId { get; set; }
        public string Sid { get; set; }
        public string Sid2 { get; set; }
        public string[] Permissions { get; set; }
        public List<Tenant> Tenants { get; set; }
    }
}
