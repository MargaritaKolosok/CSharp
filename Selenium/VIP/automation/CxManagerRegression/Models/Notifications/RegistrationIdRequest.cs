using Models.Notifications.Tags;

namespace Models.Notifications
{
    public class RegistrationIdRequest
    {
        public string Platform { get; set; }
        public string Handle { get; set; }
        public Tag Tags { get; set; }
    }
}
