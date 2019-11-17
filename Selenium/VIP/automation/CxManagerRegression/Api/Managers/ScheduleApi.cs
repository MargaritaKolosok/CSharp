using Api.Controllers;
using Api.Resources;
using Models.Places.Schedules;
using Newtonsoft.Json;
using RestSharp;

namespace Api.Managers
{
    /// <summary>
    /// Requests to CX Manager API, endpoint group ScheduleEntry
    /// </summary>
    public class ScheduleApi
    {
        /// <summary>
        /// Get new schedule entry
        /// </summary>
        /// <param name="appVersionId">App version ID</param>
        /// <returns>(<see cref="ScheduleApp"/>) Schedule entry object</returns>
        public static ScheduleApp GetScheduleEntry(long appVersionId)
        {
            var response = RestController.HttpRequestJson(
                string.Format(UriCxm.ScheduleEntry, appVersionId), Method.GET);
            return JsonConvert.DeserializeObject<ScheduleApp>(response.Content);
        }
    }
}
