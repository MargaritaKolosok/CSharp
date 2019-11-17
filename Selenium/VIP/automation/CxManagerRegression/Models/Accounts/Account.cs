using System.Collections.Generic;
using Models.Accounts.Models;
using Models.BackgroundTasks;
using Models.Tenants;

namespace Models.Accounts
{
    public class Account
    {
        public string Company { get; set; }
        public string Created { get; set; }
        public string Email { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public long? Id { get; set; }
        public List<string> Languages { get; set; }
        public string LoginPermission { get; set; }
        public List<Model> Models { get; set; }
        public List<string> Permissions { get; set; }
        public List<BackgroundTask> BackgroundTasks { get; set; }
        public string Phone { get; set; }
        public List<Tenant> Tenants { get; set; }
        public string UserEditPageUrl { get; set; }
        public string UserStatus { get; set; }
    }
}
