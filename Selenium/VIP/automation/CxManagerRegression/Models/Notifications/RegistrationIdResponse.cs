using Models.Notifications.BodyTemplates;

namespace Models.Notifications
{
    public class RegistrationIdResponse
    {
        public string ETag { get; set; }
        public string ExpirationTime { get; set; }
        public string RegistrationId { get; set; }
        public string Tags { get; set; }
        public string GcmRegistrationId { get; set; }
        public BodyTemplate BodyTemplate { get; set; }
        public string TemplateName { get; set; }
    }
}
