namespace Models.Accounts
{
    public class RegistrationInfo
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
    }
}
