using System.Collections.Generic;
using Api.Controllers;
using Api.Helpers;
using Api.Resources;
using Common.Configuration;
using Common.Managers;
using Models.Accounts;
using Models.Users;
using Newtonsoft.Json;
using RestSharp;

namespace Api.Managers
{
    /// <summary>
    /// Requests to CX Manager API, endpoint group Account
    /// </summary>
    public static class AccountApi
    {
        public static Account GetAccountData(string tenantCode = null)
        {
            var response = RestController.HttpRequestJson(UriCxm.Account, Method.GET, null, tenantCode);
            return JsonConvert.DeserializeObject<Account>(response.Content);
        }

        /// <summary>
        /// Creates a new user account for CX Manager
        /// </summary>
        /// <param name="newUser">New user dictionary</param>
        public static void CreateNewAccount(User newUser)
        {
            var headers = new Dictionary<string, string>
            {
                { "Tenant", "" },
                { "sid", null }
            };
            var parameters = new RegistrationInfo
            {
                Email = newUser.Email,
                Phone = "+666666666",
                GivenName = "Auto",
                FamilyName = $"Auto test {ActionManager.RandomNumber}",
                Company = "Ameria",
                PasswordHash = RestController.GetIntermediateHash(newUser.Password)
            };

            RestController.HttpRequestForm(TestConfig.BaseUrlApi + UriCxm.AccountRegister,
                Method.POST,
                headers,
                parameters.ToDictionary());
        }
    }
}
