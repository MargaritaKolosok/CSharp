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
            driver.
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

        [Test]

        public void CssDemo()
        {
            driver = new ChromeDriver();
            driver.Url = "http://demo.guru99.com/test/guru99home/";
            driver.Manage().Window.Maximize();
            IWebElement link = driver.FindElement(By.XPath(".//*[@id='rt-header']//div[2]/div/ul/li[2]/a"));

            link.Click();
            driver.Close();
        }       
        
    }
}
