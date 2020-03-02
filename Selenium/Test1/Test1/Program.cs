using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;
using NUnit.Framework;

namespace Test1
{
    [TestFixture]
    class TestProgram
    {
        [Test]
        public void Test()
        {
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("Google.com");
            driver.FindElement(By.Name("q")).SendKeys("Test");
            Thread.Sleep(10);
        }

    }
}
