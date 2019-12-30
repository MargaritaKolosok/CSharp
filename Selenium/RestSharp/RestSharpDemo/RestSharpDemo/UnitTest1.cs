using Amazon.SimpleSystemsManagement.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;


namespace RestSharpDemo
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var client = new RestClient("http://prototypedb-test.materialise.net/");
            var request = new RestRequest("NewPricing/api/Country", Method.GET);

            var response = client.Execute(request);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Could not get resource from REST service.");
            }
            
            var deserializer = new JsonDeserializer();
            deserializer.Deserialize<List<InventoryItem>>(response);
            Console.WriteLine(deserializer.ToString());
        }
    }
}
