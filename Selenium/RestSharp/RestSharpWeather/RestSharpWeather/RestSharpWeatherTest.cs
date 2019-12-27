using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace RestSharpWeather
{
    [TestClass]
    public class RestSharpWeatherTest
    {
        [TestMethod]
        public void GetWeatherInfo()
        {
            RestClient restClient = new RestClient("http://restapi.demoqa.com/utilities/weather/city/");
            RestRequest restRequest = new RestRequest("Kiev", Method.GET);
            IRestResponse restResponse = restClient.Execute(restRequest);
            string response = restResponse.Content;

            if (!response.Contains("Kiev"))
                {
                    Assert.Fail("Whether information is not displayed");
                }
                
        }
    }
}
