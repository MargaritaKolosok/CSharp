using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Threading;
using Newtonsoft.Json;

namespace RestSharpAuthSample
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var client = new RestClient("https://api.twitter.com/1.1");
            client.Authenticator = new HttpBasicAuthenticator("username", "password");
            var request = new RestRequest("statuses/home_timeline.json", DataFormat.Json);
            var response = client.Get(request);
            // var timeline = await client.GetAsync<HomeTimeline>(request, cancellationToken);
            Console.WriteLine(response.Content.ToString());
            Assert.AreEqual(400, (int)response.StatusCode);
        }
    }
}  