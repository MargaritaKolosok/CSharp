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
            driver.Navigate().GoToUrl("https://www.google.com");
            Console.WriteLine("Browser opened");
        }

        [Test]
        public void ExecuteTest()
        {         
            IWebElement element = driver.FindElement(By.Name("q"));
            element.SendKeys("Selenium");
            Console.WriteLine("Elements found");
        }
        [TearDown]
        public void CleanUp()
        {
            driver.Close();
            Console.WriteLine("Browser closed");
        }
    }
}
