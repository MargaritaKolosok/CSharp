using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using System.Text.Json;

namespace RestSharpDemo.Methods
{
    class GetRequest
    {
        public GetRequest(string client, string request)
        {
            var restClient = new RestClient(client);
            var restRequest = new RestRequest(request, Method.GET);
            var response = restClient.Execute(restRequest);
        }
        private void CheckResponseStatus()
        {
            //if (response.StatusCode != HttpStatusCode.OK)
            //{
            //    throw new Exception("Could not get resource from REST service.");
            //}
        }

    }
}
