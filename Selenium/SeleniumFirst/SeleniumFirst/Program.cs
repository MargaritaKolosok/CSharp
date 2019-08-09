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
           // SeleniumSetMethods.SelectDropDown(driver, "TitleId", "Mr.", "Id");
            SeleniumSetMethods.GetDropDownValue(driver, "TitleId", 1, "Id");

            // Initial
            SeleniumSetMethods.EnterText(driver, "Initial", "executeautomation", "Name");

            // Get Text from controls
            Console.WriteLine("The value of my Title is {0}", SeleniumGetMethods.GetText(driver, "TitleId", "Id"));
            Console.WriteLine("The value of my Initial is {0}", SeleniumGetMethods.GetText(driver, "Initial", "Name"));

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
