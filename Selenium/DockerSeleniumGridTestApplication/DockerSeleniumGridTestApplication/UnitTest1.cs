using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Threading;

namespace DockerSeleniumGridTestApplication
{
    [TestFixture(BrowserType.Chrome)]
    [TestFixture(BrowserType.FireFox)]
    [Parallelizable(ParallelScope.Fixtures)]
    public class Tests : Hooks
    {        
        public Tests(BrowserType _browser) : base(_browser)
        {
            
        }


        [Test]
        public void ChromeGoogleTest()
        {
            Driver.Navigate().GoToUrl("http://www.google.com");
            Driver.FindElement(By.Name("q")).SendKeys("ExecuteAutomation");

            Driver.FindElement(By.Name("q")).SendKeys(Keys.Enter);
            Thread.Sleep(5);
            Assert.That(Driver.PageSource.Contains("ExecuteAutomation"), Is.EqualTo(true),
                                            "The text ExecuteAutomation doest not exist");
        }
    }
}