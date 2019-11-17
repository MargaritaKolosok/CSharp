using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AutoIt;
using Common.Browsers;
using Common.Configuration;
using Common.Resources;
using OpenQA.Selenium;

namespace Common.Managers
{
    /// <summary>
    /// Manages browser windows, tabs, and basic actions with them
    /// </summary>
    public class WindowManager
    {
        /// <summary>
        /// Open primary browser instance of <see cref="TestConfig.PrimaryBrowser"/>
        /// type if it is not opened
        /// </summary>
        protected static void OpenPrimaryBrowser()
        {
            Browser.OpenBrowser(ref Browser.Driver, TestConfig.PrimaryBrowser);
        }

        /// <summary>
        /// Closes primary browser instance if opened
        /// </summary>
        public static void ClosePrimaryBrowser()
        {
            Browser.CloseBrowser(Browser.Driver);
        }

        /// <summary>
        /// Open primary browser instance of <see cref="TestConfig.SecondaryBrowser"/>
        /// type if it is not opened
        /// </summary>
        protected static void OpenSecondaryBrowser()
        {
            Browser.OpenBrowser(ref Browser.Driver2, TestConfig.SecondaryBrowser);
        }

        /// <summary>
        /// Closes second browser instance
        /// </summary>
        protected static void CloseSecondaryBrowser()
        {
            Browser.CloseBrowser(Browser.Driver2);
        }

        /// <summary>
        /// Opens a new browser tab of <see cref="Browser.CurrentDriver"/> and switches
        /// focus to it
        /// </summary>
        /// <returns>(<see cref="string"/>) Returns new tab handle or null if failed to open</returns>
        protected static string OpenNewTab()
        {
            if (Browser.CurrentDriver?.WindowHandles == null)
            {
                return null;
            }
            
            ((IJavaScriptExecutor) Browser.CurrentDriver).ExecuteScript("window.open();");
            var newTabHandle = Browser.CurrentDriver.WindowHandles.Last();
            SwitchToTab(newTabHandle);
            return newTabHandle;
        }

        /// <summary>
        /// Closes browser tab identified by handle on <see cref="Browser.CurrentDriver"/>,
        /// then switches focus to first available tab or closes browser if the last tab
        /// was closed
        /// </summary>
        /// <param name="tabHandle">Tab handle</param>
        protected static void CloseTab(string tabHandle)
        {
            if (Browser.CurrentDriver.WindowHandles != null
                && !string.IsNullOrEmpty(tabHandle))
            {
                Browser.CurrentDriver.SwitchTo().Window(tabHandle);
            }

            Browser.CurrentDriver.Close();

            try
            {
                Browser.CurrentDriver.SwitchTo().Window(Browser.CurrentDriver.WindowHandles[0]);                
            }
            catch
            {
                Browser.CloseBrowser(Browser.CurrentDriver);
            }          
        }

        /// <summary>
        /// Switches focus to existing tab of <see cref="Browser.CurrentDriver"/> 
        /// </summary>
        /// <param name="tabHandle">Tab handle</param>
        protected static void SwitchToTab(string tabHandle)
        {
            Browser.CurrentDriver.SwitchTo().Window(tabHandle);
        }

        /// <summary>
        /// Returns current tab handle of <see cref="Browser.CurrentDriver"/>
        /// </summary>
        /// <returns>(<see cref="string"/>) Current tab handle</returns>
        protected static string GetCurrentTabHandle()
        {
            try
            {
                return Browser.CurrentDriver?.CurrentWindowHandle;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Returns all open tabs handles collection of <see cref="Browser.CurrentDriver"/>
        /// </summary>
        /// <returns>(<see cref="ICollection{String}"/>) Tab handles collection</returns>
        protected static ICollection<string> GetTabHandles()
        {
            try
            {
                return Browser.CurrentDriver?.WindowHandles;
            }
            catch
            {
                return null;
            } 
        }

        /// <summary>
        /// Switches focus to another browser instance. This method does not open new
        /// browser window. Changes field <see cref="Browser.CurrentDriver"/> value.
        /// </summary>
        protected static void SwitchToAnotherBrowser()
        {
            if (Browser.CurrentDriver.CurrentWindowHandle == Browser.Driver.CurrentWindowHandle)
            {
                Browser.CurrentDriver = Browser.Driver2;
                Browser.CurrentDriver.SwitchTo().Window(Browser.Driver2.WindowHandles.LastOrDefault());
                while (AutoItX.WinActive(Browser.WindowHandles.LastOrDefault()) == 0)
                {
                    AutoItX.WinActivate(Browser.WindowHandles.LastOrDefault());
                }
            }
            else
            {
                Browser.CurrentDriver = Browser.Driver;
                Browser.CurrentDriver.SwitchTo().Window(Browser.Driver.WindowHandles.LastOrDefault());
                while (AutoItX.WinActive(Browser.WindowHandles.FirstOrDefault()) == 0)
                {
                    AutoItX.WinActivate(Browser.WindowHandles.FirstOrDefault());
                }
            }
        }

        /// <summary>
        /// Changes <see cref="Browser.CurrentDriver"/> window size to a specified one
        /// </summary>
        /// <param name="size">Size</param>
        protected static void ChangeWindowSize(Size size)
        {
            Browser.CurrentDriver.Manage().Window.Size = size;
        }

        /// <summary>
        /// Maximizes <see cref="Browser.CurrentDriver"/> window
        /// </summary>
        protected static void WindowMaximize()
        {
            Browser.CurrentDriver.Manage().Window.Maximize();
        }


        /// <summary>
        /// Returns current browser and tab URL
        /// </summary>
        /// <returns>(<see cref="string"/>) Current page URL or null if not applicable</returns>
        protected static string GetCurrentUrl()
        {
            try
            {
                return Browser.CurrentDriver.Url;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Refreshes page on current browser tab 
        /// </summary>
        protected static void RefreshPage()
        {
            ActionManager.CloseAlert();
            Browser.CurrentDriver.Navigate().Refresh();
            ActionManager.CloseAlert();
            ActionManager.WaitForPageReady(TestConfig.ImplicitWaitTimeout);
            ActionManager.IsElementFound(CommonElement.GlobalFader);
        }

        /// <summary>
        /// Detects whether current form is not changed, already saved or does not require
        /// save by trying current page to refresh and further looking for a javascript alert  
        /// </summary>
        /// <returns>(<see cref="bool"/>) True if saved</returns>
        protected static bool IsFormSaved()
        {
            ActionManager.CloseAlert();
            var currentWindow = GetCurrentTabHandle();
            Browser.CurrentDriver.Navigate().Refresh();
            try
            {
                Browser.CurrentDriver.SwitchTo().Alert();
                Browser.CurrentDriver.SwitchTo().Window(currentWindow);
                return false;
            }
            catch (NoAlertPresentException)
            {
                return true;
            }
        }

        /// <summary>
        /// Scrolls current web driver page to bottom
        /// </summary>
        protected static void ScrollBottom()
        {
            try
            {
                ((IJavaScriptExecutor)Browser.CurrentDriver)
                    .ExecuteScript(
                       "window.scrollTo(0, document.body.scrollHeight || document.documentElement.scrollHeight);"
                    );
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Scrolls current web driver page to top
        /// </summary>
        protected static void ScrollTop()
        {
            try
            {
                ((IJavaScriptExecutor)Browser.CurrentDriver)
                    .ExecuteScript("window.scrollTo(0, 0);");
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Checks whether a web element is visible
        /// </summary>
        /// <param name="selector">Web element selector</param>
        /// <returns>(<see cref="bool"/>) True when visible</returns>
        protected static bool IsScrolledIntoView(string selector)
        {
            return (bool)((IJavaScriptExecutor)Browser.CurrentDriver)
                .ExecuteScript(@"
                    const el = $('" + selector + @"')[0];
                    const rect = el.getBoundingClientRect();
                    const elemTop = rect.top;
                    const elemBottom = rect.bottom;
                    return (elemTop >= 0) && (elemBottom <= window.innerHeight);"
                );
            // Only completely visible elements return true:
            // return (elemTop >= 0) && (elemBottom <= window.innerHeight);
            // Partially visible elements return true:
            // return elemTop < window.innerHeight && elemBottom >= 0;"
        }
    }
}
