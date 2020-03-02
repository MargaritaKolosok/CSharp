using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace SeleniumGrid
{
    [TestFixture]
    public class Testing
    {
        private IWebDriver driver;
    
    [SetUp]
        [Obsolete]
        public void SetUp()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--ignore-certificate-errors");
            chromeOptions.AddArgument("--allow-running-insecure-content");
            chromeOptions.AddArgument("--disable-extensions");
            chromeOptions.AddArgument("--start-maximized");

            chromeOptions.AddAdditionalCapability("name", "TEST NAME"); //this gives an error when running the test
            DesiredCapabilities desiredCapabilities = new DesiredCapabilities();
            desiredCapabilities.SetCapability("idleTimeout", 150);

            desiredCapabilities.SetCapability(ChromeOptions.Capability, chromeOptions);
            driver = new RemoteWebDriver(new Uri("http://172.17.0.3:5555"), desiredCapabilities);
        }
    
    [Test]
        public void TestGoogle()
        {
            driver.Navigate().GoToUrl("http://www.google.co.uk");
            driver.FindElement(By.Name("q")).SendKeys("Test Docker");
            driver.FindElement(By.Name("q")).SendKeys(Keys.Enter);
            Thread.Sleep(5);
        }    
    [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
