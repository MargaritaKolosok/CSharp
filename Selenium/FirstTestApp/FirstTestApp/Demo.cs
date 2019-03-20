using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;

namespace FirstTestApp
{
    class Demo
    {
        IWebDriver driver;

        [SetUp]

        public void StartBrowser()
        {
            driver = new ChromeDriver("C:\\Charp_git\\CSharp\\Selenium\\FirstTestApp\\packages\\Selenium.WebDriver.ChromeDriver.73.0.3683.68\\driver\\win32");
        }
            [Test]

        public void Test()
        {
            driver.Url = "http://www.google.co.in";
        }

        [TearDown]

        public void CloseBrowser()
        {
            driver.Close();
        }
    }
}
