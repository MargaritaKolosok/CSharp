using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutomationPatternSingleTon
{
    class Test
    {
        public IWebDriver driver;

        [SetUp]
        public void startBrowser()
        {
            driver = new ChromeDriver();
        }

        [Test]
        public void test()
        {
            // driver.Url = "http://www.google.co.in";
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("http://www.google.co.in");
        }

        [TearDown]
        public void closeBrowser()
        {
           // driver.Close();
        }
    }
}
