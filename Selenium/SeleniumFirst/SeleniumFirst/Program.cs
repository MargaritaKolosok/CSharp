using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using NUnit.Framework;


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
            driver.Navigate().GoToUrl("http://google.com");
            Console.WriteLine("Opened URL");
        }

        [Test]
        public void ExecuteTest()
        {
            IWebElement element = driver.FindElement(By.Name("q"));
            element.SendKeys("Ameria AG");
            element.SendKeys(Keys.Enter);

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
