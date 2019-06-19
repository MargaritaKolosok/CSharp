using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace SeleniumFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create reference for our driver

            IWebDriver driver = new ChromeDriver();

            // Navigate to the google page
            driver.Navigate().GoToUrl("http://google.com");

            IWebElement element = driver.FindElement(By.Name("q"));

            element.SendKeys("Ameria AG");

            element.SendKeys(Keys.Enter);

        }
    }
}
