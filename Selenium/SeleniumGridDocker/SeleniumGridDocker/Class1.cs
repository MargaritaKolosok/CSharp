using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumGridDocker
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
