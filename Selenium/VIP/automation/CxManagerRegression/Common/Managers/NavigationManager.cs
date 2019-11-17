using System;
using Common.Browsers;
using Common.Configuration;
using OpenQA.Selenium;

namespace Common.Managers
{
    /// <summary>
    /// Manages navigation functionality and cache
    /// </summary>
    public class NavigationManager : WindowManager
    {
        /// <summary>
        /// Opens web page by given URL. Performs safety checks.
        /// </summary>
        /// <param name="url">Target page URL</param>
        protected static void NavigateTo(string url)
        {
            if (!url.Contains(TestConfig.BaseUrl))
            {
                Browser.CurrentDriver.Navigate().GoToUrl(url);
                return;
            }

            if (string.Equals(url, TestConfig.LoginUrl, StringComparison.OrdinalIgnoreCase))
            {
                Browser.CurrentDriver.Navigate().GoToUrl(url);
                ActionManager.CloseAlert();
                ActionManager.CloseModalDialogs();
                ActionManager.WaitForPageReady(TestConfig.ImplicitWaitTimeout);
                RefreshPage();
                return;
            }

            if (!string.Equals(Browser.CurrentDriver.Url, url, StringComparison.OrdinalIgnoreCase))
            {
                Browser.CurrentDriver.Navigate().GoToUrl(url);
                ActionManager.CloseAlert();
                ActionManager.CloseModalDialogs();
                ActionManager.WaitForPageReady(TestConfig.ImplicitWaitTimeout);
                if (url.Contains($@"{TestConfig.PlaceUri}/"))
                {                  
                    RefreshPage();
                }
            }
        }

        /// <summary>
        /// Opens web page by given URL immediately without any checks
        /// </summary>
        /// <param name="url">Target page URL</param>
        protected static void Navigate(string url)
        {
            Browser.CurrentDriver.Navigate().GoToUrl(url);
        }

        /// <summary>
        /// Clears browser cache and local storage for <see cref="Browser.CurrentDriver"/>
        /// </summary>
        protected static void ClearBrowserCache()
        {
            ActionManager.CloseAlert();
            try
            {
                Browser.CurrentDriver.Manage().Cookies.DeleteAllCookies();
                ((IJavaScriptExecutor)Browser.CurrentDriver)
                    .ExecuteScript(
                        "window.sessionStorage.clear(); window.localStorage.clear();"
                    );
            }
            catch
            {
                // ignored
            }
        }
    }
}
