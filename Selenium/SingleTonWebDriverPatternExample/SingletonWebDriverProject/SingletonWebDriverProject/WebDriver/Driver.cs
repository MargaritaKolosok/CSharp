using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Reflection;

namespace SingletonWebDriverProject.WebDriver
{
    public static class Driver
    {
        private static WebDriverWait _browserWait;
        private static IWebDriver _browser;
        public static IWebDriver Browser
        {
            get
            {
                if (_browser == null)
                {
                    throw new NullReferenceException("The WebDriver browser instance was not initialized. You should first call the method Start.");
                }
                return _browser;
            }
            private set => _browser = value;
        }
        public static WebDriverWait BrowserWait
        {
            get
            {
                if (_browserWait == null || _browser == null)
                {
                    throw new NullReferenceException("The WebDriver browser wait instance was not initialized. You should first call the method Start.");
                }
                return _browserWait;
            }
            private set => _browserWait = value;
        }
        public static void StartBrowser(BrowserType browserType = BrowserType.Chrome, int defaultTimeOut = 30)
        {
            switch (browserType)
            {
                case BrowserType.Firefox:
                    Browser = new FirefoxDriver();
                    break;
                case BrowserType.InternetExplorer:
                    break;
                case BrowserType.Chrome:
                    Browser = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                    break;
                default:
                    throw new ArgumentException("You need to set a valid browser type.");
            }
            BrowserWait = new WebDriverWait(Browser, TimeSpan.FromSeconds(defaultTimeOut));
        }
        public static void StopBrowser()
        {
            Browser.Quit();
            Browser = null;
            BrowserWait = null;
        }
        public static string GetCurrentURL()
        {
            return Browser.Url.ToString();
        }
    }
    public enum BrowserType
    {
        Chrome,
        Firefox,
        InternetExplorer
    }
}