using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using Api.Controllers;
using Api.Managers;
using Api.Resources;
using Common.Configuration;
using Common.Managers;
using Models.UserDirectory;
using Models.Users;
using Newtonsoft.Json;
using RestSharp;

namespace Api.Helpers
{
    /// <summary>
    /// Gets and updates user SIDs for CX Manager and User Directory web apps.
    /// Also populates tenants list.
    /// </summary>
    internal static class SidsTenants
    {
        internal const string UdApplicationName = "vipUserDirectory";
        internal const string CxmApplicationName = "vipb";
        private static readonly object O = new object();

        /// <summary>
        /// Gets user SID for CX Manager web services and available tenants 
        /// Results are stored to private properties Sid and Tenants.
        /// </summary>
        /// <param name="user">User object</param>
        internal static void GetSidAndTenants(User user)
        {
            var myIp = RestController.MyIp();
            var formattedTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var body = new AuthorizationRequest
            {
                Email = user.Email,
                DateTime = formattedTime,
                Password = RestController.GetSessionHash(formattedTime, user.Password),
                StayLoggedIn = true,
                ClientIp = myIp
            };

            var response = RestController.HttpRequestJsonUnauthenticated(
                UriCxm.AccountLogin, Method.POST, body);
            var responseJson = JsonConvert.DeserializeObject<AuthorizationResponse>(response.Content);

            user.Id = responseJson.UserId;
            user.Sid = responseJson.Sid;

            ActionManager.Tenants = new ConcurrentBag<Tenant>(UserDirectoryApi.GetTenantList());
        }

        /// <summary>
        /// Gets user SID for User Directory web services
        /// </summary>
        private static void GetSidUserDirectory()
        {
            var formattedTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var parameters = new Dictionary<string, string>
            {
                { "Email", TestConfig.AdminUser.Email },
                { "DateTime", formattedTime },
                { "Password", RestController.GetSessionHash(formattedTime, TestConfig.AdminUser.Password) },
                { "lang", "en_US" }
            };

            var request = new RestRequest(TestConfig.UserDirectoryBaseUrlApi + UriUserDirectory.AccountLogin,
                Method.POST);
            request.AddHeader("appname", UdApplicationName);
            request.Timeout = TestConfig.ApiRequestTimeout;
            foreach (var parameter in parameters)
            {
                request.AddParameter(parameter.Key, parameter.Value);
            }

            var client = new RestClient(TestConfig.UserDirectoryBaseUrlApi);
            var response = client.Execute(request);

            // when no response from service, try one more time
            if (response.StatusCode == 0)
            {
                response = client.Execute(request);
            }

            if (response.StatusCode == 0 || (int) response.StatusCode >= (int) HttpStatusCode.BadRequest)
            {
                throw new WebException(
                    $"User Directory: HTTP {(int) response.StatusCode} {response.StatusCode} error\n" +   
                    $@"in request {response.Request.Method} {TestConfig.UserDirectoryBaseUrlApi}{UriUserDirectory.AccountLogin}" +
                    $"\nERROR:\n{response.Content}");
            }

            var responseJson = JsonConvert.DeserializeObject<Login>(response.Content);
            UserDirectoryApi.Headers["sid"] = TestConfig.AdminUser.SidUd = responseJson.Sid;
        }

        /// <summary>
        /// Updates UD user SID every 29 min
        /// </summary>
        internal static void GetSidUd()
        {
            lock (O)
            {
                if (DateTime.Now - UserDirectoryApi.SidLastUpdateTime < TimeSpan.FromMinutes(29)
                        && !string.IsNullOrEmpty(TestConfig.AdminUser.SidUd))
                {
                    return;
                }
                // refresh SID for admin user
                GetSidUserDirectory();
                UserDirectoryApi.SidLastUpdateTime = DateTime.Now;
                UserDirectoryApi.Headers["sid"] = TestConfig.AdminUser.SidUd; 
            }
        }
    }
}
