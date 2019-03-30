
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;

namespace SeleniumTutorialY1
{
    class Program
    {
        

        static void Main(string[] args)
        {            
            
        }

        [SetUp]
        public void Initialize()
        {
            PropertiesCollection.driver = new ChromeDriver();
            PropertiesCollection.driver.Navigate().GoToUrl("http://www.executeautomation.com/demosite/index.html");
            Console.WriteLine("URL opened");
        }

        [Test]
        public void ExecuteTest()
        {
            // Title
            SeleniumSetMethods.SelectDropDown("TitleId", "Mr.", "Id");

            // Initial
            SeleniumSetMethods.EnterText("Initial", "Executeautomation", "Name");

            Console.WriteLine("The value from my Title is: " + SeleniumGetMethods.GetTextFromDDL(driver, "TitleId", "Id"));
            Console.WriteLine("The value from Initial is: " + SeleniumGetMethods.GetText(driver, "Initial", "Name"));

            // Click
            SeleniumSetMethods.Click("Save", "Name");

            Console.WriteLine("Select value in dropdown");
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Close();
            Console.WriteLine("Browser closed");
        }
    }
}
