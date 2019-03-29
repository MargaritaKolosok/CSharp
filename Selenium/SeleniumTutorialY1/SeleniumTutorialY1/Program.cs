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
        IWebDriver driver = new ChromeDriver();

        static void Main(string[] args)
        {            
        }

        [SetUp]
        public void Initialize()
        {
            driver.Navigate().GoToUrl("http://www.executeautomation.com/demosite/index.html");
            Console.WriteLine("URL opened");
        }

        [Test]
        public void ExecuteTest()
        {
            // Title
            SeleniumSetMethods.SelectDropDown(driver, "TitleId", "Mr.", "Id");

            // Initial
            SeleniumSetMethods.EnterText(driver, "Initial", "Executeautomation", "Name");

            // Click
            SeleniumSetMethods.Click(driver, "Save", "Name");

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
