using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using NUnit.Framework;
using OpenQA.Selenium.Support.UI;


namespace SeleniumFirst
{
    class Program
    {
        // Create reference for our driver
        IWebDriver driver = new ChromeDriver();

        static void Main(string[] args)
        {           

        }

        [SetUp]
        public void Initialize()
        {
            // Navigate to the google page
            driver.Navigate().GoToUrl("http://www.executeautomation.com/demosite/index.html");
            Console.WriteLine("Opened URL");
        }

        [Test]
        public void ExecuteTest()
        {
            // Title
            SeleniumSetMethods.SelectDropDown(driver, "TitleId", "Mr.", "Id");

            // Initial
            SeleniumSetMethods.EnterText(driver, "Initial", "executeautomation", "Name");

            // Click
            SeleniumSetMethods.Click(driver, "Save", "Name");

            Console.WriteLine("Executed test");
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Close();

            Console.WriteLine("Browser closed");
        }

    }
}
