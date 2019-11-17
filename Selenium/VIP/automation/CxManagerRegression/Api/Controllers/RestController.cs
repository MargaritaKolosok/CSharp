using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Api.Helpers;
using Api.Resources;
using Common.Configuration;
using Common.Managers;
using Models.Users;
using RestSharp;

namespace Api.Controllers
{
    /// <summary>
    /// RESTful API controller to provide data exchange with CX Manager and User Directory
    /// web services
    /// </summary>
    public static class RestController
    {
        /// <summary>
        /// The last user used for CX Manager service API requests
        /// </summary>
        private static User _lastUser;
        private static readonly object O = new object();
        private const string Salt = "-vipComposer";

        /// <summary>
        /// Get initial user SID for CX Manager web service and available tenants list
        /// </summary>
        /// <returns>(<see cref="Task"/>) Task object</returns>
        public static Task StartClientAsync()
        {
            return Task.Run(() => { CheckUser(TestConfig.AdminUser); });
        }

        /// <summary>
        /// Checks whether specified <paramref name="user"/> has been initialized or
        /// changed since the last API request and setups it if necessary. Follow up
        /// requests to CX Manager web service will be made on behalf of the
        /// <paramref name="user"/>.
        /// </summary>
        private static void CheckUser(User user)
        {
            lock (O)
            {
                if (_lastUser != null 
                    && _lastUser.Email == user.Email 
                    && !string.IsNullOrEmpty(_lastUser.Sid)
                    && !string.IsNullOrEmpty(user.Sid))
                {
                    return;
                }

                SidsTenants.GetSidAndTenants(user);
                _lastUser = user;
            }
        }

        /// <summary>
        /// Server API response validator that throws exception when response contains HTTP error
        /// </summary>
        /// <exception cref="AggregateException">When HTTP response is unsuccessful</exception>
        /// <param name="response">Raw response from server (see <see cref="IRestResponse"/>)</param>
        private static void ResponseValidator(IRestResponse response)
        {
            if (response.IsSuccessful)
            {
                return;
            }

            throw new AggregateException(
                $"HTTP {(int) response.StatusCode} {response.StatusCode} \n" +
                $"in request {response.Request.Method} {response.ResponseUri} \n " +
                $"{response.Request.Parameters.SingleOrDefault(x => x.Name.ToLower() == "tenant")} \n" +
                $"ERROR: \n{response.Content}");
        }

        /// <summary>
        /// Makes HTTP(S) request authenticated by SID to the CxM web service</summary>
        /// <param name="uri">Endpoint URI without domain name and leading slash</param>
        /// <param name="method">HTTP request method (see <see cref="Method"/>)</param>
        /// <param name="body">JSON formatted request body (optional)</param>
        /// <param name="tenantCode">Tenant code (optional, uses <see cref=
        /// "ActionManager.CurrentTenantCode"/> value by default)</param>
        /// <param name="user">User to make request (optional, uses <see cref=
        /// "ActionManager.CurrentUser"/> by default)</param>
        /// <returns>(<see cref="IRestResponse"/>) Response from web service</returns>
        internal static IRestResponse HttpRequestJson(string uri, Method method, object body = null,
            string tenantCode = null, User user = null)
        {
            user = user ?? ActionManager.CurrentUser;
            CheckUser(user);
            var request = new RestRequest(uri, method);
            request.AddHeader("sid", user.Sid);
            request.AddHeader("Tenant", tenantCode ?? ActionManager.CurrentTenantCode);
            request.Timeout = TestConfig.ApiRequestTimeout;
            if (body != null)
            {
                request.AddJsonBody(body);
            }
            
            var client = new RestClient(TestConfig.BaseUrlApi)
                .UseSerializer(() => new JsonNetSerializer());
            var response = client.Execute(request);
            ResponseValidator(response);

            return response;
        }

        /// <summary>
        /// Makes a HTTP(S) request authenticated by App ID and API Key to CxM web services</summary>
        /// <param name="uri">Endpoint URI without domain name and leading slash</param>
        /// <param name="method">HTTP request method (see <see cref="Method"/>)</param>
        /// <param name="body">JSON string or any other JSON object (optional)</param>
        /// <param name="appId">App ID (see app package)</param>
        /// <param name="apiKey">API Key (see app package)</param>
        /// <param name="tenantCode">Tenant code (optional, uses <see cref=
        /// "ActionManager.CurrentTenantCode"/> value by default)</param>
        /// <returns>(<see cref="IRestResponse"/>) Response from web service or null if request
        /// has failed</returns>
        internal static IRestResponse HttpRequestJsonByAppId(string uri, Method method, object body,
            string appId, string apiKey, string tenantCode = null)
        {
            var request = new RestRequest(TestConfig.BaseUrlApi + uri, method);
            request.AddHeader("app-id", appId);
            request.AddHeader("api-key", apiKey);
            request.AddHeader("Tenant", tenantCode ?? ActionManager.CurrentTenantCode);
            request.Timeout = TestConfig.ApiRequestTimeout;
            
            if (body != null)
            {
                // to avoid string body serialization
                request.AddParameter("application/json", body, ParameterType.RequestBody);
            }

            var client = new RestClient(TestConfig.BaseUrlApi)
                .UseSerializer(() => new JsonNetSerializer());
            var response = client.Execute(request);

            // when no response from service, try one more time
            if (response.StatusCode == 0)
            {
                response = client.Execute(request);
            }

            return response.IsSuccessful ? response : null;
        }

        /// <summary>
        /// Makes an unauthenticated HTTP(S) request to the CxM web service</summary>
        /// <param name="uri">Endpoint URI without domain name and leading slash</param>
        /// <param name="method">HTTP request method (see <see cref="Method"/>)</param>
        /// <param name="body">JSON formatted request body (optional)</param>
        /// <returns>(<see cref="IRestResponse"/>) Response from web service</returns>
        internal static IRestResponse HttpRequestJsonUnauthenticated(string uri, Method method,
            object body = null)
        {
            var request = new RestRequest(TestConfig.BaseUrlApi + uri, method);
            if (body != null)
            {
                request.AddJsonBody(body);
            }
            request.Timeout = TestConfig.ApiRequestTimeout;

            var client = new RestClient(TestConfig.BaseUrlApi)
                .UseSerializer(() => new JsonNetSerializer());
            var response = client.Execute(request);

            // when no response from service, try one more time
            if (response.StatusCode == 0)
            {
                response = client.Execute(request);
            }

            ResponseValidator(response);

            return response;
        }

        /// <summary>
        /// Makes a form-data HTTP(S) request to any web service</summary>
        /// <param name="url">Endpoint full URL</param>
        /// <param name="method">HTTP request method (see <see cref="Method"/>)</param>
        /// <param name="headers">Additional request header parameters (optional)</param>
        /// <param name="parameters">Form parameters in request (optional)</param>
        /// <returns>(<see cref="string"/>) Response body</returns>
        internal static string HttpRequestForm(
            string url,
            Method method,
            IDictionary<string, string> headers = null,
            IDictionary<string, object> parameters = null)
        {
            SidsTenants.GetSidUd();
            var request = new RestRequest(url, method)
            {
                Timeout = TestConfig.ApiRequestTimeout
            };

            if (headers != null && headers.Count > 0)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            if (parameters != null && parameters.Count > 0)
            {
                foreach (var parameter in parameters)
                {
                    if (parameter.Key != null)
                    {
                        request.AddParameter(parameter.Key, parameter.Value);
                    }
                }
            }

            var client = new RestClient(TestConfig.UserDirectoryBaseUrlApi);
            var response = client.Execute(request);

            // when no response from service, try one more time
            if (response.StatusCode == 0)
            {
                response = client.Execute(request);
            }

            ResponseValidator(response);
            return response.Content;
        }

        /// <summary>
        /// Makes an HTTP(S) request to User Directory web service</summary>
        /// <param name="uri">Endpoint URI without domain name and leading slash</param>
        /// <param name="method">HTTP request method (see <see cref="Method"/>)</param>
        /// <param name="headers">Additional request header parameters (optional)</param>
        /// <param name="body">JSON formatted request body (optional)</param>
        /// <returns>(<see cref="string"/>) Response body</returns>
        internal static string HttpRequestJsonUserDirectory(string uri, Method method,
            IDictionary<string, string> headers = null, object body = null)
        {
            var request = new RestRequest(TestConfig.UserDirectoryBaseUrlApi + uri, method)
            {
                Timeout = TestConfig.ApiRequestTimeout
            };
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }
            if (body != null)
            {
                request.AddJsonBody(body);
            }

            var client = new RestClient(TestConfig.UserDirectoryBaseUrlApi)
                .UseSerializer(() => new JsonNetSerializer());
            var response = client.Execute(request);

            // when no response from service, try one more time
            if (response.StatusCode == 0)
            {
                response = client.Execute(request);
            }

            ResponseValidator(response);

            return response.Content;
        }

        /// <summary>
        /// Calculates password hash for login
        /// </summary>
        /// <param name="formattedTime">Formatted time string</param>
        /// <param name="password">Password string</param>
        /// <returns>(<see cref="string"/>) User session hash</returns>
        internal static string GetSessionHash(string formattedTime, string password)
        {
            return ToSha3Hash($"{GetIntermediateHash(password)}-{MyIp()}-{formattedTime}");
        }

        /// <summary>
        /// Calculates password hash for account creation or save
        /// </summary>
        /// <param name="password">Password string</param>
        /// <returns>(<see cref="string"/>) Password hash</returns>
        internal static string GetIntermediateHash(string password)
        {
            return ToSha3Hash(password + Salt);
        }

        /// <summary>
        /// Returns current connection IP address
        /// </summary>
        /// <returns>(<see cref="string"/>) IP address</returns>
        internal static string MyIp()
        {
            var resp = HttpRequestJsonUnauthenticated(UriCxm.AccountMyIp, Method.GET);
            return resp.Content.Trim('\"');
        }

        /// <summary>
        /// Gets SHA3 key for authentication
        /// </summary>
        /// <param name="value">Value used to generate a key</param>
        /// <returns>(<see cref="string"/>) SHA3 key</returns>
        private static string ToSha3Hash(string value)
        {
            var keccak = new Keccak(1600);
            var temp = BitConverter.ToString(Encoding.UTF8.GetBytes(value).ToArray())
                .Replace("-", string.Empty);

            return keccak.GetHash(temp, 1088, 512, 32).ToLower();
        }

        /// <summary>
        /// Returns MD5 hash for specified stream
        /// </summary>
        /// <param name="stream">Stream instance</param>
        /// <returns>(<see cref="string"/>) MD5 hash</returns>
        internal static string GetMd5Hash(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(stream);
                return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
            }
        }
    }
}