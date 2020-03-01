using Amazon.SimpleSystemsManagement.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using RestSharp.Serialization.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace RestSharpDemo
{
    public class Country
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool EU { get; set; }
        public double Factor { get; set; }

    }

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
            List<Country> Countries = new List<Country>();
            Countries = deserializer.Deserialize<List<Country>>(response);

            foreach (var country in Countries)
            {
                if (country.Name != "" && country.Code != "")
                {
                    Console.WriteLine(country.Name);
                }

                else
                {
                    throw new Exception("Empty Country Name or Code value");
                }

            }

        }
    }
}
