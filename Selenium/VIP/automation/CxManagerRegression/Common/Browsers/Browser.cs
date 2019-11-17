using System;
using System.Collections.Generic;
using System.Linq;
using Common.Configuration;
using Common.Enums;
using Common.Managers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Opera;
using OpenQA.Selenium.Safari;

namespace Common.Browsers
{
    /// <summary>
    /// Opens and closes browser applications
    /// </summary>
    internal static class Browser
    {
        internal static IWebDriver Driver, Driver2, CurrentDriver;
        internal static readonly List<IntPtr> WindowHandles = new List<IntPtr>();

        /// <summary>
        /// Opens new browser window by browser type and sets web driver object up
        /// </summary>
        /// <param name="driver">Driver object. Will be setup at the end of the method.</param>
        /// <param name="browserType">Browser type (see <see cref="BrowserType"/>)</param>
        internal static void OpenBrowser(ref IWebDriver driver, BrowserType browserType)
        {
            if (driver != null)
            {
                CurrentDriver = driver;
                ActionManager.CloseAlert();
                return;
            }
            switch (browserType)
            {
                case BrowserType.Chrome:
                case BrowserType.ChromeIncognito:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArguments("disable-infobars", "disable-geolocation", "disable-gpu", "no-sandbox");
                    chromeOptions.AddUserProfilePreference("download.default_directory", TestConfig.BrowserDownloadFolder);
                    if (browserType == BrowserType.ChromeIncognito)
                    {
                        chromeOptions.AddArgument("incognito");
                    }
                    driver = new ChromeDriver(chromeOptions);
                    break;
                case BrowserType.Firefox:
                    var ffOptions = new FirefoxOptions();
                    ffOptions.SetPreference("app.update.auto", false);
                    ffOptions.SetPreference("app.update.enabled", false);
                    ffOptions.SetPreference("app.update.silent", false);
                    ffOptions.SetPreference("geo.enabled", false);
                    ffOptions.SetPreference("geo.provider.use_corelocation", false);
                    ffOptions.SetPreference("geo.prompt.testing", false);
                    ffOptions.SetPreference("geo.prompt.testing.allow", false);
                    ffOptions.SetPreference("browser.download.folderList", 2);
                    ffOptions.SetPreference("browser.download.dir", TestConfig.BrowserDownloadFolder);
                    ffOptions.SetPreference("browser.download.manager.showWhenStarting", false);
                    ffOptions.SetPreference("browser.helperApps.neverAsk.saveToDisk", "image/jpeg");
                    ffOptions.SetPreference("layers.acceleration.disabled", true);
                    driver = new FirefoxDriver(ffOptions);
                    break;
                // Cannot be used due to browser GUI "open file" dialog calls from CX Manager.
                // AutoItX cannot control invisible dialogs.
                //
                //case BrowserType.ChromeHeadless:
                //    chromeOptions.AddArguments("headless", "disable-gpu", "disable-infobars", "window-size=1920x1080");
                //    driver = new ChromeDriver(chromeOptions);
                //    break;
                case BrowserType.InternetExplorer:
                    driver = new InternetExplorerDriver();
                    break;
                case BrowserType.Edge:
                    driver = new EdgeDriver();
                    break;
                case BrowserType.Safari:
                    driver = new SafariDriver();
                    break;
                case BrowserType.Opera:
                    driver = new OperaDriver();
                    break;
                default:
                    throw new WebDriverException("Unsupported browser type is set in TestConfig.xlsx");
            }

            driver.SwitchTo().DefaultContent();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(TestConfig.ImplicitWaitTimeout);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(TestConfig.PageLoadWaitTimeout);
            CurrentDriver = driver;
            WindowHandles.Add(AutoIt.AutoItX.WinGetHandle());
        }

        /// <summary>
        /// Closes corresponding browser window given by web driver parameter
        /// </summary>
        /// <param name="driver">Web driver object that represents a browser instance to be closed</param>
        internal static void CloseBrowser(IWebDriver driver)
        {
            if (driver == null)
            {
                return;
            }

            if (driver == Driver2 && Driver2 != null)
            {
                try
                {
                    Driver2.SwitchTo().Window(Driver2.WindowHandles.LastOrDefault());
                }
                catch
                {
                    // ignored
                }
                CurrentDriver = Driver2;
                ActionManager.CloseAlert();
                Driver2.Quit();
                WindowHandles.Remove(WindowHandles.LastOrDefault());
                Driver2.Dispose();
                Driver2 = null;
                CurrentDriver = Driver;
                return;
            }

            if (driver == Driver && Driver != null)
            {
                try
                {
                    Driver.SwitchTo().Window(Driver.WindowHandles.FirstOrDefault());
                }
                catch
                {
                    // ignored
                }
                CurrentDriver = Driver;
                ActionManager.CloseAlert();
                ActionManager.IsUserLoggedIn = false;
                Driver.Quit();
                WindowHandles.Remove(WindowHandles.FirstOrDefault());
                Driver.Dispose();
                Driver = null;
                CurrentDriver = null;
            }
        }
    }
}
