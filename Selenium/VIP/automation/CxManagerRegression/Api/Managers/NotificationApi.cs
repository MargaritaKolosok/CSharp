using System.Net;
using Api.Controllers;
using Api.Resources;
using Common.Managers;
using Models.Notifications;
using Models.Notifications.Tags;
using Newtonsoft.Json;
using RestSharp;

namespace Api.Managers
{
    /// <summary>
    /// Requests to CX Manager API, endpoint group Notifications
    /// </summary>
    public static class NotificationApi
    {
        /// <summary>
        /// Creates registration ID for notification
        /// </summary>
        /// <param name="handle">Device application UUID</param>
        /// <param name="appId">iBeacon app ID</param>
        /// <param name="apiKey">iBeacon app key</param>
        /// <returns>(string) Registration ID</returns>
        public static string CreateRegistrationId(string handle, string appId, string apiKey)
        {
            var response = RestController.HttpRequestJsonByAppId(
                string.Format(UriCxm.NotificationsCreate, handle),
                Method.POST,
                null,
                appId,
                apiKey);
            
            return response?.Content?.Replace("\"", string.Empty);
        }

        /// <summary>
        /// Determines whether notification ID registration successful or not
        /// </summary>
        /// <param name="registrationId">Registration ID</param>
        /// <param name="handle">Device application UUID</param>
        /// <param name ="appId"> iBeacon app ID</param>
        /// <param name="apiKey">iBeacon app key</param>
        /// <returns>(bool) True if registration successful</returns>
        public static bool IsRegistrationIdResponseValid(string registrationId, string handle, string appId, string apiKey)
        {
            var registrationRequestBody = new RegistrationIdRequest
            {
                Platform = "gcm",
                Handle = handle,
                Tags = new Tag { lang = "en", tenant = ActionManager.CurrentTenantCode }
            };
            
            var data = Newtonsoft.Json.Linq.JObject.FromObject(registrationRequestBody).ToString();

            var response = RestController.HttpRequestJsonByAppId(
                string.Format(UriCxm.NotificationsRegister, registrationId),
                Method.POST,
                data,
                appId,
                apiKey);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            try
            {
                var a = JsonConvert.DeserializeObject<RegistrationIdResponse>(response.Content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether notification ID deleted successfully or not
        /// </summary>
        /// <param name="registrationId">Registration ID</param>
        /// <param name ="appId"> iBeacon app ID</param>
        /// <param name="apiKey">iBeacon app key</param>
        /// <returns>(bool) True if deleted successfully</returns>
        public static bool IsRegistrationDeleted(string registrationId, string appId, string apiKey)
        {
            var response = RestController.HttpRequestJsonByAppId(
                string.Format(UriCxm.NotificationsDeleteRegistration, registrationId),
                Method.DELETE,
                null,
                appId,
                apiKey);

            return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.NoContent;
        }
    }
}
