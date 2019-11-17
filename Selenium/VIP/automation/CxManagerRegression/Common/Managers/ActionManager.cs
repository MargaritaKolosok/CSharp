using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using Common.Browsers;
using Common.Configuration;
using Common.Enums;
using Common.Resources;
using Models.UserDirectory;
using Models.Users;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Keys = OpenQA.Selenium.Keys;

namespace Common.Managers
{
    /// <summary>
    /// The main web element actions class. Provides simple actions.
    /// </summary>
    public class ActionManager : NavigationManager
    {
        /// <summary>
        /// Collection of all active tenants with properties
        /// </summary>
        public static ConcurrentBag<Tenant> Tenants = new ConcurrentBag<Tenant>();
        public static string RandomNumber => GenerateRandomNumber();
        public static string RandomNumberWord => GenerateRandomUint16();
        /// <summary>
        /// Current user used in CX Manager UI and API requests. Does NOT affect User Directory.
        /// </summary>
        public static User CurrentUser = new User();
        /// <summary>
        /// Current tenant title
        /// </summary>
        public static TenantTitle CurrentTenant
        {
            get => (TenantTitle) (int) Enum.Parse(typeof(TenantCode), CurrentTenantCode, true);
            set
            {
                CurrentTenantCode = Tenants.Count > 0
                    ? Tenants.Single(x => x.Title == value.ToString()).Code
                    : ((TenantCode) (int) value).ToString();
            }
        }
        /// <summary>
        /// Tenant code of current tenant. This property is set automatically by
        /// <see cref="CurrentTenant"/>.
        /// </summary>
        public static string CurrentTenantCode { get; private set; }
        /// <summary>
        /// Property that shows whether some user is currently logged in
        /// </summary>
        public static bool IsUserLoggedIn { protected get; set; }
        /// <summary>
        /// Whether all check(s) should be used when system waits for current page
        /// to be ready
        /// </summary>
        public static bool IsUseAllPageReadyChecks = true;
        /// <summary>
        /// How often Selenium WebDriver will check conditions
        /// </summary>
        private static readonly TimeSpan PollingInterval = TimeSpan.FromMilliseconds(250);

        private static readonly RNGCryptoServiceProvider GlobalRandom = 
            new RNGCryptoServiceProvider((CspParameters) null);
        private static readonly RNGCryptoServiceProvider GlobalRandom2 =
            new RNGCryptoServiceProvider(new CspParameters());
        [ThreadStatic]
        private static Random _threadRandom, _threadRandom2;

        /// <summary>
        /// Thread-safe 15-digit random number generator
        /// </summary>
        /// <returns>(<see cref="string"/>) Random number as string</returns>
        private static string GenerateRandomNumber()
        {
            var random = _threadRandom;
            if (random == null)
            {
                var buffer = new byte[4];
                GlobalRandom.GetBytes(buffer);
                _threadRandom = random = new Random(BitConverter.ToInt32(buffer, 0));
            }
            var buf = new byte[8];
            random.NextBytes(buf);
            return (Math.Abs(BitConverter.ToInt64(buf, 0) % 899_999_999_999_999) + 100_000_000_000_000).ToString();
        }

        /// <summary>
        /// Thread-safe random <see cref="ushort"/> numbers generator
        /// </summary>
        /// <returns>(<see cref="string"/>) Random number as string</returns>
        private static string GenerateRandomUint16()
        {
            var random = _threadRandom2;
            if (random == null)
            {
                var buffer = new byte[2];
                GlobalRandom2.GetBytes(buffer);
                _threadRandom2 = random = new Random(BitConverter.ToInt16(buffer, 0));
            }
            var buf = new byte[4];
            random.NextBytes(buf);
            return Math.Abs(BitConverter.ToInt16(buf, 0) % 65535).ToString();
        }

        /// <summary>
        /// Finds element on web page or throws exception when not found
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="timeout">Timeout or 0 for default implicit wait timeout (optional)</param>
        /// <param name="noScroll">Do not scroll window and element (optional)</param>
        /// <param name="ignoreResult">Do not throw exception if element not found (optional)</param>
        /// <returns>(<see cref="IWebElement"/>) Web element or null if not found</returns>
        private static IWebElement FindElement(string selector, 
            double timeout = 0, 
            bool noScroll = false,
            bool ignoreResult = false)
        {
            if (Math.Abs(timeout) < PollingInterval.TotalSeconds)
            {
                timeout = TestConfig.ImplicitWaitTimeout;
            }

            var isNoScroll = noScroll.ToString().ToLower();

            var wait = new WebDriverWait(
                Browser.CurrentDriver,
                TimeSpan.FromSeconds(timeout))
            {
                PollingInterval = PollingInterval
            };

            WaitForPageReady(timeout);
            try
            {
                return wait.Until(driver =>
                {
                    var elem = (IWebElement) ((IJavaScriptExecutor) driver).ExecuteScript(@"
                        const element = $('" + selector + @"')[0];
                        if (typeof element !== 'undefined') { 
                            const elementRect = element.getBoundingClientRect();                          
                            if (elementRect.height > 0) {
                                if (!" + isNoScroll + @") {
                                    const absoluteElementTop = elementRect.top + window.pageYOffset;
                                    const middle = absoluteElementTop - (window.innerHeight / 2);
                                    window.scrollTo(0, middle); 
                                }
                                return element;
                            }
                        }
                        else {
                            return null;
                        }");
                    // stale element check
                    try
                    {
                        return !elem.Size.IsEmpty && elem.Displayed ? elem : null;
                    }
                    catch
                    {
                        return null; // stale element found, find and check it again
                    }
                });
            }
            catch (Exception ex)
            {
                if (!ignoreResult)
                {
                    throw new Exception(
                        $"{ex.Message} Element not found. Selector: $('{selector}').eq(0)");
                }

                return null;
            }
        }

        /// <summary>
        /// Find all elements on web page or throws exception when not found
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="timeout">Timeout or 0 for default implicit timeout (optional)</param>
        /// <returns>(<see cref="IEnumerable{IWebElement}"/>) Collection of web elements</returns>
        private static IEnumerable<IWebElement> FindElements(string selector, double timeout = 0)
        {
            if (Math.Abs(timeout) < PollingInterval.TotalSeconds)
            {
                timeout = TestConfig.ImplicitWaitTimeout;
            }
            var wait = new WebDriverWait(
                Browser.CurrentDriver,
                TimeSpan.FromSeconds(timeout))
            {
                PollingInterval = PollingInterval
            };
            WaitForPageReady(timeout);
            try
            {
                return wait.Until(driver =>
                {
                    var elemCollection = (ICollection<IWebElement>) ((IJavaScriptExecutor) driver)
                        .ExecuteScript(@"
                            const elements = $('" + selector + @"');
                            if (elements.length > 0) { 
                                return Array.from(elements);
                            }
                            else {
                                return null;
                            }");
                    try
                    {
                        // stale or no elements check
                        foreach (var elem in elemCollection.Reverse())
                        {
                            if (elem.Size.IsEmpty || !elem.Displayed)
                            {
                            }
                        }
                        return elemCollection; 
                    }
                    catch
                    {
                        return null; // stale element found, find elements again
                    }
                });
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{ex.Message} Elements collection not found. Selector: $('{selector}')");
            }
        }

        /// <summary>
        /// Checks whether element found during implicit wait timeout.
        /// Implicit wait is set in config TestConfig.xlsx.
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="timeout">Timeout or 0 for default implicit timeout (optional)</param>
        /// <returns>(<see cref="bool"/>) True if found</returns>
        public static bool IsElementFound(string selector, double timeout = 0)
        {
            if (Math.Abs(timeout) < PollingInterval.TotalSeconds)
            {
                timeout = TestConfig.ImplicitWaitTimeout;
            }

            var wait = new WebDriverWait(
                Browser.CurrentDriver,
                TimeSpan.FromSeconds(timeout))
            {
                PollingInterval = PollingInterval
            };

            WaitForPageReady(timeout);
            try
            {
                return wait.Until(driver => (bool) ((IJavaScriptExecutor) driver)
                    .ExecuteScript(@"
                        const element = $('" + selector + @"')[0];
                        if (typeof element !== 'undefined') { 
                            return true;
                        }
                        else {
                            return false;
                        }"
                    ));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether element is found in a short period of time. Default
        /// timeout is set in config TestConfig.xlsx.
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="timeout">Element search timeout in seconds (optional)</param>
        /// <returns>(<see cref="bool"/>) True if found</returns>
        protected static bool IsElementFoundQuickly(string selector, double timeout = 0)
        {
            if (Math.Abs(timeout) < PollingInterval.TotalSeconds)
            {
                timeout = TestConfig.CheckIfElementExistsTimeout;
            }

            var wait = new WebDriverWait(
                Browser.CurrentDriver,
                TimeSpan.FromSeconds(timeout))
            {
                PollingInterval = PollingInterval
            };
            WaitForPageReady(timeout);
            try
            {
                return wait.Until(driver => (bool) ((IJavaScriptExecutor) driver)
                    .ExecuteScript(@"
                        const element = $('" + selector + @"')[0];
                        if (typeof element !== 'undefined') {
                            return true;
                        }
                        else {
                            return false;
                        }"
                    ));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether element is NOT found during implicit wait timeout.
        /// Implicit wait is set in config TestConfig.xlsx.
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="timeout">Timeout or 0 for default implicit timeout (optional)</param>
        /// <returns>(<see cref="bool"/>) True if NOT found</returns>
        protected static bool IsElementNotFound(string selector, double timeout = 0)
        {
            if (Math.Abs(timeout) < PollingInterval.TotalSeconds)
            {
                timeout = TestConfig.ImplicitWaitTimeout;
            }

            var wait = new WebDriverWait(
                Browser.CurrentDriver,
                TimeSpan.FromSeconds(timeout))
            {
                PollingInterval = PollingInterval
            };
            
            WaitForPageReady(timeout);
            try
            {
                return wait.Until(driver => (bool) ((IJavaScriptExecutor) driver)
                    .ExecuteScript(@"
                        const element = $('" + selector + @"')[0];
                        if (typeof element === 'undefined') {
                            return true;
                        }
                        else {
                            return false;
                        }"
                    ));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether element is NOT found in a short period of time. 
        /// Default timeout is set in config TestConfig.xlsx.
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="timeout">Element search timeout in seconds (optional)</param>
        /// <returns>(<see cref="bool"/>) True if NOT found</returns>
        protected static bool IsElementNotFoundQuickly(string selector, double timeout = 0)
        {
            if (Math.Abs(timeout) < PollingInterval.TotalSeconds)
            {
                timeout = TestConfig.CheckIfElementDoesNotExistTimeout;
            }

            var wait = new WebDriverWait(
                Browser.CurrentDriver,
                TimeSpan.FromSeconds(timeout))
            {
                PollingInterval = PollingInterval
            };

            WaitForPageReady(timeout);
            try
            {
                return wait.Until(driver => (bool) ((IJavaScriptExecutor) driver)
                    .ExecuteScript(@"
                        const element = $('" + selector + @"')[0];
                        if (typeof element === 'undefined') {
                            return true;
                        }
                        else {
                            return false;
                        }")
                );
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether current page and its components are loaded and ready, 
        /// other transformations have stopped. Throws exception if
        /// <paramref name="timeout"/> expired.
        /// </summary>
        /// <exception cref="WebDriverTimeoutException"/>
        /// <param name="timeout">Wait timeout in seconds. It cannot be shorter
        /// than <see cref="TestConfig.ImplicitWaitTimeout"/> and set to this
        /// value by default.</param>
        public static void WaitForPageReady(double timeout = 0)
        {
            if (timeout < TestConfig.ImplicitWaitTimeout)
            {
                timeout = TestConfig.ImplicitWaitTimeout;
            }

            var additionalCheck = string.Empty;
            if (IsUseAllPageReadyChecks)
            {
                additionalCheck = 
                    @" && $('" + CommonElement.UploadSpinner + @"').length === 0";
            }

            var script = @"
                if (document.readyState !== 'complete') {
                    return false; 
                }
                if (typeof ng === 'undefined' || ng.probe(document.body) === null) {
                    return false; 
                }
                if ($('" + CommonElement.Main + @"').length === 0) {
                    return false; 
                }
                const conditions = $('" + CommonElement.PageLoader + @"').length === 0
                    && $('" + CommonElement.UploadImageProgress + @"').length === 0
                    && $('" + CommonElement.Fader + @"').length === 0"
                + additionalCheck + @";
                const globalFader = document.querySelector('" + CommonElement.GlobalFader + @"');
                let globalFaderOpacity = 0;
                if (globalFader !== null) { 
                    globalFaderOpacity = globalFader.style.opacity;
                }
                if (conditions && !globalFaderOpacity) {
                    return true;
                }
                else {
                    return false;
                }";

            var wait = new WebDriverWait(Browser.CurrentDriver,
                TimeSpan.FromSeconds(timeout))
            {
                PollingInterval = PollingInterval
            };

            try
            {
                wait.Until(driver => (bool) ((IJavaScriptExecutor) driver).ExecuteScript(script));
            }
            catch (Exception ex)
            {
                throw new WebDriverTimeoutException($"Page ready timeout. \n{ex.Message}");
            }
        }

        /// <summary>
        /// Send text to web element
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="text">Text to send</param>
        /// <param name="isIgnoreError">Whether any web element issues ignored or not (optional)
        /// </param>
        /// <param name="isCheckInput">Whether inputted text should be checked and, if it is
        /// wrong, inputted one more time (optional)</param>
        protected void SendText(string selector, string text, bool isIgnoreError = false, bool isCheckInput = false)
        {
            try
            {
                var element = FindElement(selector, noScroll: true);
                element.Clear();
                element.SendKeys(text);
                if (isCheckInput)
                {
                    if (GetValue(selector) != text)
                    {
                        Thread.Sleep(1000);
                        element.Clear();
                        Thread.Sleep(1000);
                        element.SendKeys(text);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!isIgnoreError)
                {
                    throw new Exception(
                        $"{ex.Message} Cannot send text to element. Selector: $('{selector}').eq(0)");
                }
            }
        }

        /// <summary>
        /// Transmits a key sequence to current browser tab
        /// </summary>
        /// <param name="keySequence">Key sequence</param>
        protected void PressKeys(string keySequence)
        {
            WaitForPageReady(TestConfig.ImplicitWaitTimeout);
            new Actions(Browser.CurrentDriver).SendKeys(keySequence).Perform();
        }

        /// <summary>
        /// Holds specified key and mouse-clicks specified web element
        /// </summary>
        /// <param name="key">Key to be held during mouse-click</param>
        /// <param name="selector">Web element selector</param>
        protected void HoldKeyAndClick(string selector, string key)
        {
            var element = FindElement(selector);
            new Actions(Browser.CurrentDriver)
                .MoveToElement(element)
                .KeyDown(key)
                .Click()
                .KeyUp(key)
                .Build()
                .Perform();
        }

        /// <summary>
        /// Clear all text in currently focused element
        /// </summary>
        protected void ClearTextInFocusedElement()
        {
            ClearText();
        }

        /// <summary>
        /// Clear all text in element
        /// </summary>
        /// <param name="selector">Element selector</param>
        protected void ClearTextInElement(string selector)
        {
            Click(selector);
            ClearText(selector);
        }

        /// <summary>
        /// Calls key sequence to clear text
        /// </summary>
        /// <param name="selector">Element selector</param>
        private void ClearText(string selector = "")
        {
            try
            {
                new Actions(Browser.CurrentDriver)
                    .SendKeys(Keys.End)
                    .KeyDown(Keys.LeftShift)
                    .SendKeys(Keys.Home)
                    .KeyUp(Keys.LeftShift)
                    .SendKeys(Keys.Backspace)
                    .Build()
                    .Perform();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{ex.Message} Cannot clear field {(!string.IsNullOrEmpty(selector) ? $@"$('{selector}').eq(0)" : "focused")}");
            }
        }

        /// <summary>
        /// Clicks on visible web element.
        /// Do NOT use this method unless you suspect that required element cannot receive a click 
        /// because the element is currently not visible or another element overlaps or hides it.
        /// Resulting Selenium exception might give you a hint.
        /// </summary>
        /// <param name="selector">Element selector</param>
        protected void Click2(string selector)
        {
            try
            {
                FindElement(selector).Click();
            }
            catch (Exception ex)
            {
                throw new WebDriverException(
                    $@"{ex.Message} Cannot click element. Selector: $('{selector}').eq(0)");
            }
        }

        /// <summary>
        /// Performs click web element action
        /// </summary>
        /// <param name="ignoreIfNoElement">Do not throw exception if click fails (optional)</param>
        protected static void ClickAction(bool ignoreIfNoElement = false)
        {
            var wait = new WebDriverWait(Browser.CurrentDriver,
                TimeSpan.FromSeconds(TestConfig.CheckIfElementExistsTimeout))
            {
                PollingInterval = PollingInterval
            };
            try
            {
                wait.Until(x =>
                {
                    try
                    {
                        new Actions(Browser.CurrentDriver).Click().Perform();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                if (!ignoreIfNoElement)
                {
                    throw new WebException($"{ex.Message} Cannot click element.");
                }
            }
        }

        /// <summary>
        /// Performs web element click
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="ignoreIfNoElement">Do not throw exception if click fails (optional)</param>
        /// <param name="timeout">Timeout or 0 for default implicit wait timeout (optional)</param>
        /// <param name="noScroll">Do not scroll window and element (optional)</param>
        protected static void Click(
            string selector, 
            bool ignoreIfNoElement = false, 
            double timeout = 0,
            bool noScroll = false)
        {
            if (Math.Abs(timeout) < PollingInterval.TotalSeconds)
            {
                timeout = TestConfig.ImplicitWaitTimeout;
            }

            try
            {
                TurnOffInfoPopups();
                try
                {
                    IWebElement elem = null;
                    var wait = new WebDriverWait(
                        Browser.CurrentDriver,
                        TimeSpan.FromSeconds(timeout - 2));
                    wait.Until(x =>
                    {
                        elem = FindElement(selector, timeout, noScroll, ignoreResult: true);
                        return elem != null;
                    });
                    elem.Click();                    
                }
                catch
                {
                    var isSuccess = MouseOver(selector, 2, noScroll, ignoreIfNoElement);
                    if (isSuccess)
                    {
                        ClickAction(ignoreIfNoElement);
                    }
                }
            }
            catch (Exception ex)
            {
                if (ignoreIfNoElement)
                {
                    return;
                }
                throw new WebException(
                    $"{ex.Message} Cannot click element. Selector: $('{selector}').eq(0)");
            }
        }

        /// <summary>
        /// Clicks a specified element at given coordinates related to an origin
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="offsetX">X coordinate related to an origin</param>
        /// <param name="offsetY">Y coordinate related to an origin</param>
        /// <param name="origin">Origin (center or top left corner) to apply given coordinates (optional)</param>
        protected void ClickAtPoint(
            string selector, 
            int offsetX, 
            int offsetY, 
            MoveToElementOffsetOrigin origin = MoveToElementOffsetOrigin.TopLeft)
        {
            try
            {
                MouseOver(selector, offsetX, offsetY, origin);
                ClickAction();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{ex.Message} Cannot click element. Selector: $('{selector}').eq(0)");
            }
        }

        /// <summary>
        /// Performs web element click until another web element becomes visible during
        /// <see cref="TestConfig.ImplicitWaitTimeout"/> timeout
        /// </summary>
        /// <param name="selectorToClick">Selector of a web element to be clicked</param>
        /// <param name="selectorToBeShown">Selector of a web element which should be shown</param>
        /// <param name="timeout">How many seconds to wait for appearance of an element with selector
        /// specified by <paramref name="selectorToBeShown"/> after every click (optional)</param>
        protected void ClickUntilShown(string selectorToClick, string selectorToBeShown, double timeout = 1)
        {
            try
            {
                var wait = new WebDriverWait(Browser.CurrentDriver,
                    TimeSpan.FromSeconds(TestConfig.ImplicitWaitTimeout))
                {
                    PollingInterval = PollingInterval
                };
                wait.Until(driver =>
                {
                    Click(selectorToClick, ignoreIfNoElement: true, 5);
                    return !IsElementNotFound(selectorToBeShown, timeout);
                });
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"{ex.Message} Cannot click element with selector $('{selectorToClick}').eq(0) or element $('{
                        selectorToBeShown}').eq(0) is not shown");
            }
        }

        /// <summary>
        /// Performs web element click until specified condition is met
        /// </summary>
        /// <param name="selectorToClick">Selector of a web element to be clicked</param>
        /// <param name="condition">Condition under which web element clicking must stop</param>
        protected void ClickUntilConditionMet(string selectorToClick, Func<bool> condition)
        {
            try
            {              
                var wait = new WebDriverWait(Browser.CurrentDriver,
                    TimeSpan.FromSeconds(TestConfig.ImplicitWaitTimeout))
                {
                    PollingInterval = PollingInterval
                };
                wait.Until(driver =>
                {
                    Click(selectorToClick, ignoreIfNoElement: true, 2);
                    return condition();
                });
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"{ex.Message} Cannot click element with selector $('{selectorToClick}').eq(0) or " +
                    $@"condition '{condition.ToString()}' is false");
            }
        }

        /// <summary>
        /// Opens drop-down list and selects list item by specified value
        /// </summary>
        /// <param name="selectorToClick">Drop-down element selector</param>
        /// <param name="valueToSelect">Value to be selected in item list</param>
        /// <param name="isCheckValue">Check whether the right value is displayed in drop-down
        /// (optional)</param>
        protected void DropDownSelect(string selectorToClick, string valueToSelect, bool isCheckValue = true)
        {
            try
            {
                var selector = string.IsNullOrEmpty(valueToSelect)
                    ? CommonElement.DropDownOptionEmpty
                    : string.Format(CommonElement.DropDownOption, valueToSelect);
                var wait = new WebDriverWait(Browser.CurrentDriver,
                    TimeSpan.FromSeconds(TestConfig.ImplicitWaitTimeout))
                {
                    PollingInterval = PollingInterval
                };
      
                wait.Until(driver =>
                {
                    Click(selectorToClick, 
                        ignoreIfNoElement: true,
                        TestConfig.CheckIfElementExistsTimeout);
                    // Sometimes dropdown list of options closes itself, but the 
                    // script manages to find this element and throws exception when 
                    // it closes. Therefore we need this pause before its processing.
                    Thread.Sleep(1000);
                    if (IsElementFoundQuickly(CommonElement.DropDownOptionList, 0.5))
                    { 
                        // scroll to specified value within options list
                        var result = (bool) ((IJavaScriptExecutor) driver)
                            .ExecuteScript(@"
                                const option = $('" + selector + @"')[0];
                                if (typeof option === 'undefined') {
                                    return false;
                                }
                                const topPos = option.offsetTop;
                                let optionList = $('" + CommonElement.DropDownOptionList + @"')[0];
                                optionList.scrollTop = topPos;
                                return true;");
                        if (!result)
                        {
                            return false;
                        }

                        Click(selector,
                            ignoreIfNoElement: true,
                            TestConfig.CheckIfElementExistsTimeout,
                            noScroll: true);                     
                        
                        if (!isCheckValue)
                        {
                            // used in Filter dropdown
                            return true;
                        }

                        // depending on valueToSelect,
                        // check whether drop-down is empty or contains valueToSelect 
                        return string.IsNullOrEmpty(valueToSelect)
                            ? IsElementEquals(selectorToClick, string.Empty) 
                            : AreElementsContainText(selectorToClick, valueToSelect);
                    }

                    // return to drop-down click if option list is not opened
                    return false;
                });
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"{ ex.Message} Cannot click drop-down with selector: $('{selectorToClick}').eq(0) " +
                    $@"or value '{valueToSelect}' not found in option list");
            }
        }

        /// <summary>
        /// Puts mouse pointer over an element
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="timeout">Timeout in seconds (optional)</param>
        /// <param name="noScroll">Do not scroll window and element (optional)</param>
        /// <param name="ignoreResult">Do not throw exception if mouse-over failed
        /// (optional)</param>
        /// <returns>(<see cref="bool"/>) Returns true if no exception on mouse-over</returns>
        protected static bool MouseOver(string selector, double timeout = 0, 
            bool noScroll = false, bool ignoreResult = false)
        {
            try
            {
                IWebElement element;
                if (Math.Abs(timeout) < PollingInterval.TotalSeconds)
                {
                    timeout = TestConfig.ImplicitWaitTimeout;
                }

                var wait = new WebDriverWait(
                    Browser.CurrentDriver, TimeSpan.FromSeconds(timeout))
                {
                    PollingInterval = PollingInterval
                };
                return wait.Until(driver =>
                {
                    element = FindElement(selector, timeout, noScroll, ignoreResult);
                    try
                    {
                        if (element.Size.IsEmpty)
                        {
                            return false;
                        }
                        new Actions(Browser.CurrentDriver)
                            .MoveToElement(element)
                            .Perform();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                });             
            }
            catch (Exception ex)
            {
                if (!ignoreResult)
                {
                    throw new Exception(
                        $"{ex.Message} Cannot mouse-over an element - no element or empty. " + 
                        $@" Selector: $('{selector}').eq(0)"); 
                }

                return false;
            }
        }

        /// <summary>
        /// Puts mouse pointer over specified coordinates of web element in relation to origin
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="offsetX">X coordinate</param>
        /// <param name="offsetY">Y coordinate</param>
        /// <param name="origin">Origin center or top left corner to apply offset</param>
        protected void MouseOver(
            string selector, 
            int offsetX, 
            int offsetY, 
            MoveToElementOffsetOrigin origin = MoveToElementOffsetOrigin.TopLeft)
        {
            try
            {
                var element = FindElement(selector);
                var elementSize = element.Size;
                if (elementSize.IsEmpty 
                    || elementSize.Height < offsetX - 1
                    || elementSize.Width < offsetY - 1)
                {
                    throw new ArgumentException(
                        $@"Element too small or empty. Selector: $('{selector}').eq(0)");
                }
                new Actions(Browser.CurrentDriver)
                    .MoveToElement(element, offsetX, offsetY, origin)
                    .Perform();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{ex.Message} Cannot mouse-over an element. Selector: $('{selector}').eq(0)");
            }
        }

        /// <summary>
        /// Drag-and-drops HTML5 web element using jQuery. Currently Selenium does not support
        /// 'drag*' and 'drop' browser events.
        /// </summary>
        /// <param name="sourceElementSelector">Selector of source element</param>
        /// <param name="destinationElementSelector">Selector of destination element</param>
        /// <param name="compareAttr">An attribute whose values should equal for both web
        /// elements (optional)</param>
        protected void DragAndDropHtml5(string sourceElementSelector, string destinationElementSelector,
            string compareAttr = "")
        {
            WaitForPageReady(TestConfig.ImplicitWaitTimeout);
            try
            {
                const string dragAndDropHelper =
                    @"(function($) {
                        $.fn.simulateDragDrop = function(options) {
                            return this.each(function() {
                                new $.simulateDragDrop(this, options);
                            });
                        };
                        $.simulateDragDrop = function(elem, options) {
                            this.options = options;
                            this.simulateEvent(elem, options);
                        };
                        $.extend($.simulateDragDrop.prototype, {
                            simulateEvent: function(elem, options) {
                                let targetAttrValue, elemAttrValue;
                                if (options.compareAttr)
                                { 
                                    targetAttrValue = $(options.dropTarget)[0].getAttribute(options.compareAttr);
                                    elemAttrValue = elem.getAttribute(options.compareAttr);
                                }

                                let type = 'dragstart';
                                let event = this.createEvent(type);
                                this.dispatchEvent(elem, type, event);

                                type = 'dragenter';
                                let dragEnterEvent = this.createEvent(type, {});
                                dragEnterEvent.dataTransfer = event.dataTransfer;
                                this.dispatchEvent($(options.dropTarget)[0], type, dragEnterEvent);

                                type = 'dragover';
                                let dragOverEvent = this.createEvent(type, {});
                                dragOverEvent.dataTransfer = event.dataTransfer;
                                this.dispatchEvent($(options.dropTarget)[0], type, dragOverEvent);

                                if (targetAttrValue === elemAttrValue)
                                {
                                    type = 'drop';
                                    let dropEvent = this.createEvent(type, {});
                                    dropEvent.dataTransfer = event.dataTransfer;
                                    this.dispatchEvent($(options.dropTarget)[0], type, dropEvent);
                                }

                                type = 'dragend';
                                let dragEndEvent = this.createEvent(type, {});
                                dragEndEvent.dataTransfer = event.dataTransfer;
                                this.dispatchEvent(elem, type, dragEndEvent);
                            },
                            createEvent: function(type) {
                                let event = document.createEvent('CustomEvent');
                                event.initCustomEvent(type, true, true, null);
                                event.dataTransfer = {
                                    data: { },
                                    setData: function(type, val) {
                                        this.data[type] = val;
                                    },
                                    getData: function(type) {
                                        return this.data[type];
                                    }
                                };
                                return event;
                            },
                            dispatchEvent: function(elem, type, event) {
                                if (elem.dispatchEvent) {
                                    elem.dispatchEvent(event);
                                } else if (elem.fireEvent) {
                                    elem.fireEvent('on' + type, event);
                                }
                            }
                        });
                    })(jQuery);";

                ((IJavaScriptExecutor) Browser.CurrentDriver).ExecuteScript(
                    string.Format("{0}$('{1}').simulateDragDrop({{dropTarget: '{2}', compareAttr: '{3}'}});", 
                        dragAndDropHelper, sourceElementSelector, destinationElementSelector, compareAttr));
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Problem with HTML5 drag-and-drop. {ex.Message}");
            }
        }
        
        /// <summary>
        /// Drag-and-drops source web element to destination web element's center point
        /// </summary>
        /// <param name="sourceElementSelector">Selector of source element</param>
        /// <param name="destinationElementSelector">Selector of destination element</param>
        protected void DragAndDrop(string sourceElementSelector, string destinationElementSelector)
        {
            DragAndDrop(sourceElementSelector, new Point(0, 0), destinationElementSelector);
        }

        /// <summary>
        /// Drag-and-drops source web element by specified point to destination web element's center point
        /// </summary>
        /// <param name="sourceElementSelector">Selector of source element</param>
        /// <param name="pointToDragBy">Point of source element which is used to drag the element</param>
        /// <param name="destinationElementSelector">Selector of destination element</param>
        protected void DragAndDrop(string sourceElementSelector, Point pointToDragBy, string destinationElementSelector)
        {
            try
            {
                var sourceElement = FindElement(sourceElementSelector);
                var srcElemLocation = GetElementLocation(sourceElementSelector);
                var srcElemSize = GetElementSize(sourceElementSelector);
                var dstElemLocation = GetElementLocation(destinationElementSelector);
                var dstElemSize = GetElementSize(destinationElementSelector);
                var offset = new Point(
                    dstElemLocation.X + dstElemSize.Width / 2 - srcElemLocation.X + srcElemSize.Width / 2 + pointToDragBy.X, 
                    dstElemLocation.Y + dstElemSize.Height / 2 - srcElemLocation.Y + srcElemSize.Height / 2 + pointToDragBy.Y);

                new Actions(Browser.CurrentDriver)
                    .MoveToElement(sourceElement, pointToDragBy.X, pointToDragBy.Y, MoveToElementOffsetOrigin.Center)
                    .ClickAndHold()
                    .MoveByOffset(offset.X + 20, offset.Y + 20)
                    .MoveByOffset(-40, -40)
                    .MoveByOffset(20, 20)
                    .Release()
                    .Build()
                    .Perform();

                //new Actions(Browser.CurrentDriver)
                //    .MoveToElement(sourceElement, pointToDragBy.X, pointToDragBy.Y, MoveToElementOffsetOrigin.Center)
                //    .Click(sourceElement)
                //    .ClickAndHold()
                //    .MoveByOffset(-1, -1)
                //    .MoveToElement(destinationElement, 20, 20, MoveToElementOffsetOrigin.Center)
                //    .MoveToElement(destinationElement, -20, -20, MoveToElementOffsetOrigin.Center)
                //    .MoveToElement(destinationElement, 0, 0, MoveToElementOffsetOrigin.Center)
                //    .Release()
                //    .Build()
                //    .Perform();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $@"{ex.Message}. Cannot drag-and-drop element selector: $('{sourceElementSelector}').eq(0) " +
                    $@"to place selector: $('{destinationElementSelector}').eq(0)");
            }
        }

        /// <summary>
        /// Drags element's point by given offset to another screen point. May be used to resize web elements.
        /// </summary>
        /// <param name="selector">Web element selector</param>
        /// <param name="pointToDrag">Element's top left corner offset point</param>
        /// <param name="pointOffset">Offset point in relation to <paramref name="pointToDrag"/></param>
        protected void DragAndDropByOffset(string selector, Point pointToDrag, Point pointOffset)
        {
            var element = FindElement(selector);
            try
            {
                if (pointToDrag.IsEmpty)
                {
                    new Actions(Browser.CurrentDriver)
                        .MoveToElement(element)
                        .ClickAndHold()
                        .MoveByOffset(pointOffset.X, pointOffset.Y)
                        .Release()
                        .Build()
                        .Perform();
                    return;
                }
                new Actions(Browser.CurrentDriver)
                    .MoveToElement(element, pointToDrag.X, pointToDrag.Y)
                    .ClickAndHold()
                    .MoveByOffset(pointOffset.X, pointOffset.Y)
                    .Release()
                    .Build()
                    .Perform();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{ex.Message}. Cannot drag-and-drop element selector: $('{selector}').eq(0) " +
                    $"from element offset ({pointToDrag.X}, {pointToDrag.Y}) to new offset " +
                    $"({pointOffset.X}, {pointOffset.Y})"
                );
            }
        }

        /// <summary>
        /// Gets element upper-left corner coordinates in relation to upper left corner of the page
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="isNoScroll">Do not scroll window and element (optional)</param>
        /// <returns>(<see cref="Point"/>) Element upper-left corner X and Y coordinates</returns>
        protected Point GetElementLocation(string selector, bool isNoScroll = false)
        {
            try
            {
                var element = FindElement(selector, noScroll: isNoScroll);
                return element.Location;
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{ex.Message} Cannot get element location. Selector: $('{selector}').eq(0)");
            }
        }

        /// <summary>
        /// Checks whether web element is focused at the moment
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <returns>(<see cref="bool"/>) True if element has a focus</returns>
        protected bool IsFocused(string selector)
        {
            var wait = new WebDriverWait(Browser.CurrentDriver,
                TimeSpan.FromSeconds(TestConfig.CheckIfElementDoesNotExistTimeout))
            {
                PollingInterval = PollingInterval
            };
            try
            {
                return wait.Until(driver => (bool)((IJavaScriptExecutor)driver)
                    .ExecuteScript(@"
                        const noPageLoader = $('" + CommonElement.PageLoader + @"').length === 0;
                        if (noPageLoader) {
                            return $('" + selector + @"').eq(0).is(':focus');
                        }
                        else {
                            return false;
                        }"
                    )
                );
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether web element is a default element by class "default" lookup
        /// </summary>
        /// <param name="selector">Web element selector</param>
        /// <returns>(<see cref="bool"/>) True if default element</returns>
        protected bool IsDefaultElement(string selector)
        {
            try
            {
                return FindElement(selector).GetAttribute("class").Contains("default");
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"{ex.Message} Element doesn't have attribute 'class'. Selector: $('{selector}').eq(0)");
            }
        }

        /// <summary>
        /// Checks whether web element disabled by class "disabled" lookup
        /// </summary>
        /// <param name="selector">Web element selector</param>
        /// <returns>(<see cref="bool"/>) True if element is disabled</returns>
        protected bool IsDisabledElement(string selector)
        {
            try
            {
                return FindElement(selector).GetAttribute("class").Contains("disabled");
            }
            catch (Exception ex)
            {
                throw new InvalidElementStateException(
                    $"{ex.Message} Element doesn't have attribute 'class'. Selector: $('{selector}').eq(0)");
            }
        }

        /// <summary>
        /// Get value or text from web element
        /// </summary>
        /// <param name="selector">Web element selector</param>
        /// <param name="waitForValue">Wait before element text/value analysis. Use for fields which filled
        /// by async web requests (optional, false by default).</param>
        /// <returns>(<see cref="string"/>) Value or text</returns>
        protected string GetValue(string selector, bool waitForValue = false)
        {
            var elem = FindElement(selector);
            if (waitForValue)
            {
                Thread.Sleep(1500);
            }
            try
            {
                var value = (string) ((IJavaScriptExecutor) Browser.CurrentDriver)
                    .ExecuteScript($@"return $('{selector}').eq(0).text();");
                value = value?.Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
                value = (string) ((IJavaScriptExecutor) Browser.CurrentDriver)
                    .ExecuteScript($@"return $('{selector}').eq(0).val();");
                value = value?.Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
                return GetValue(elem);
            }
            catch
            {
                return GetValue(elem);
            }
        }

        /// <summary>
        /// Get all values or texts from all web elements and their children
        /// and concatenate these values in one string
        /// </summary>
        /// <param name="selector">Web element selector</param>
        /// <returns>(<see cref="string"/>) Concatenated result string</returns>
        protected string GetValuesAsString(string selector)
        {
            if (!IsElementFound(selector))
            {
                throw new WebDriverException(
                    $@"Web elements set with selector $('{selector}') not found");
            }
            var value = (string)((IJavaScriptExecutor)Browser.CurrentDriver)
                .ExecuteScript(@"return $('" + selector + @"').val();");
            if (!string.IsNullOrEmpty(value))
            {
                return value.Trim();
            }

            value = (string)((IJavaScriptExecutor)Browser.CurrentDriver)
                .ExecuteScript(@"return $('" + selector + @"').text();");
            if (!string.IsNullOrEmpty(value))
            {
                return value.Trim();
            }

            var element = FindElement(selector);
            return GetValue(element);
        }

        /// <summary>
        /// Gets text/value from all web elements found by specified selector
        /// </summary>
        /// <param name="selector">Web elements selector</param>
        /// <returns>(<see cref="ICollection{String}"/>) Texts collection</returns>
        private ICollection<string> GetTexts(string selector)
        {
            var elements = FindElements(selector);
            var texts = new List<string>();
            foreach (var element in elements)
            {
                var text = GetValue(element);
                if (string.IsNullOrEmpty(text))
                {
                    texts.Add(string.Empty);
                    continue;
                }
                texts.Add(text);
            }

            return texts;
        }

        /// <summary>
        /// Get all values or texts from all web elements and their children
        /// and stores them as List&lt;string&gt;
        /// </summary>
        /// <param name="selector">Web elements selector</param>
        /// <returns>(<see cref="ICollection{String}"/>) Result values collection</returns>
        protected ICollection<string> GetValuesAsList(string selector)
        {
            var wait = new WebDriverWait(
                Browser.CurrentDriver, TimeSpan.FromSeconds(TestConfig.ImplicitWaitTimeout))
            {
                PollingInterval = PollingInterval
            };
            try
            {
                return wait.Until(driver => GetTexts(selector));
            }
            catch 
            {
                return new List<string>();    
            }
        }

        /// <summary>
        /// Get value or text from web element
        /// </summary>
        /// <param name="element">Web element</param>
        /// <returns>(<see cref="string"/>) Element value, text, placeholder value, or empty string if no value</returns>
        private string GetValue(IWebElement element)
        {
            string value;
            try
            {
                value = element?.Text?.Trim() ?? string.Empty;
            }
            catch
            {
                return null;
            }
        
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
            try
            {
                value = element?.GetAttribute("value")?.Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
            catch
            {
                // ignore
            }

            var placeholderValue = element != null ? element.GetAttribute("placeholder") : string.Empty;
            return !string.IsNullOrEmpty(placeholderValue) ? placeholderValue.Trim() : string.Empty;
        }

        /// <summary>
        /// Checks whether web element text/value equals exactly to <paramref name="originalValue"/>
        /// during timeout (see <see cref="TestConfig.TextValueWaitTimeout"/>)
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="originalValue">Value to compare with</param>
        /// <returns>(<see cref="bool"/>) True if equal</returns>
        protected bool IsElementEquals(string selector, string originalValue)
        {
            var wait = new WebDriverWait(
                Browser.CurrentDriver, TimeSpan.FromSeconds(TestConfig.TextValueWaitTimeout))
            {
                PollingInterval = PollingInterval
            };
            try
            {
                return wait.Until(x => GetValue(selector) == originalValue);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks two collections equality. By default, order of elements is not taken into account.
        /// </summary>
        /// <param name="collection1">First collection</param>
        /// <param name="collection2">Second collection</param>
        /// <param name="isComparePrecisely">Whether order of elements is taken into account or not (optional)</param>
        /// <returns>(<see cref="bool"/>) True when equal</returns>
        protected bool AreCollectionsEqual(IEnumerable<string> collection1, IEnumerable<string> collection2, 
            bool isComparePrecisely = false)
        {
            if (isComparePrecisely)
            {
                return collection1.SequenceEqual(collection2);
            }

            var gc1 = collection1.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            var gc2 = collection2.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            return gc1.Count == gc2.Count && gc1.All(x => gc2.ContainsKey(x.Key) && x.Value == gc2[x.Key]);
        }

        /// <summary>
        /// Checks whether <paramref name="collection1"/> contains <paramref name="collection2"/>
        /// </summary>
        /// <param name="collection1">First collection</param>
        /// <param name="collection2">Second collection</param>
        /// <returns>(<see cref="bool"/>) True if contains</returns>
        protected bool IsCollectionContainsCollection(IEnumerable<string> collection1, IEnumerable<string> collection2)
        {
            return collection2.All(collection1.Contains);
        }

        /// <summary>
        /// Checks whether all web elements contain specified text
        /// </summary>
        /// <param name="selector">Elements selector</param>
        /// <param name="text">Text to find</param>
        /// <returns>(<see cref="bool"/>) True if all elements contain the text</returns>
        protected bool AreAllElementsContainText(string selector, string text)
        {
            var elements = GetValuesAsList(selector);
            return elements.All(x => x.Contains(text));
        }

        /// <summary>
        /// Checks whether current page URL equals to target page URL
        /// </summary>
        /// <param name="targetPage">Target URL to compare</param>
        /// <returns>(<see cref="bool"/>) True if current page URL equals to target URL</returns>
        protected bool IsPageRedirectedTo(string targetPage)
        {
            var wait = new WebDriverWait(Browser.CurrentDriver,
                TimeSpan.FromSeconds(TestConfig.CheckIfPageRedirectedTimeout))
            {
                PollingInterval = PollingInterval
            };
            try
            {
                return wait.Until(x => x.Url == targetPage);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether current page URL contains specified URI
        /// </summary>
        /// <param name="uri">URI to find</param>
        /// <param name="doNotWait">Whether to wait for a page is being redirected</param>
        /// <returns>(<see cref="bool"/>) True if found</returns>
        protected bool IsPageContainsUri(string uri, bool doNotWait = false)
        {
            var wait = new WebDriverWait(Browser.CurrentDriver,
                TimeSpan.FromSeconds(doNotWait ? 1 : TestConfig.CheckIfPageRedirectedTimeout))
            {
                PollingInterval = PollingInterval
            };
            try
            {
                return wait.Until(x => x.Url.Contains(uri));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Turns off info pop-ups and field tooltips which appear downside on page.
        /// This works until page URL is changed or page refreshed.
        /// </summary>
        public static void TurnOffInfoPopups()
        {
            if (Browser.CurrentDriver == null)
            {
                return;
            }

            try
            {
                WaitForPageReady(TestConfig.ImplicitWaitTimeout);
                ((IJavaScriptExecutor) Browser.CurrentDriver)
                    .ExecuteScript(
                        $@"document.querySelector('{CommonElement.InfoPopup}').style.display = 'none';");
            }
            catch 
            {
                // ignored
            }
        }

        /// <summary>
        /// Counts web elements found by a specified selector
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <returns>(<see cref="int"/>) Number of found elements (or 0 if
        /// not found)</returns>
        protected int CountElements(string selector)
        {
            var elements = FindElements(selector);
            return elements.Count();
        }

        /// <summary>
        /// Checks whether any of web elements contains specific text
        /// </summary>
        /// <param name="selector">Elements selector</param>
        /// <param name="text">Text to find within the element</param>
        /// <returns>(<see cref="bool"/>) True if at least one element contains the text</returns>
        protected bool AreElementsContainText(string selector, string text)
        {
            WaitForPageReady(TestConfig.ImplicitWaitTimeout);

            var wait = new WebDriverWait(
                Browser.CurrentDriver, TimeSpan.FromSeconds(TestConfig.TextValueWaitTimeout))
            {
                PollingInterval = PollingInterval
            };
            try
            {
                wait.Until(driver =>
                {
                    var textsCollection = GetTexts(selector);
                    return textsCollection.Any() && textsCollection.Any(x => x.Contains(text));
                });
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether any of web elements text/value equals to specific text
        /// </summary>
        /// <param name="selector">Elements selector</param>
        /// <param name="text">Text to find within the element</param>
        /// <param name="waitForText">Wait before element text/value analysis. Use for fields
        /// which filled by async web requests (optional, false by default).</param>
        /// <returns>(<see cref="bool"/>) True if at least one element equals the text</returns>
        protected bool IsAnyElementEquals(string selector, string text, bool waitForText = false)
        {
            WaitForPageReady(TestConfig.ImplicitWaitTimeout);
            if (waitForText)
            {
                Thread.Sleep(1000);
            }

            var wait = new WebDriverWait(
                Browser.CurrentDriver, TimeSpan.FromSeconds(TestConfig.ImplicitWaitTimeout))
            {
                PollingInterval = PollingInterval
            };
            try
            {
                var textsCollection = wait.Until(driver => GetTexts(selector));
                return textsCollection.Any()
                       && textsCollection.Any(x => x == text);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether element collection is alphabetically by elements text or reverse alphabetically
        /// sorted 
        /// </summary>
        /// <param name="selector">Elements selector</param>
        /// <param name="isElementDropDown">Is elements shown as drop-down (optional, true by default)</param>
        /// <param name="isReverseOrder">Should reverse sort order be applied or not (optional, false
        /// by default)</param>
        /// <returns>(<see cref="bool"/>) True if sorted correctly</returns>
        protected bool IsAlphabeticallySorted(string selector, bool isElementDropDown = true, bool isReverseOrder = false)
        {
            var elements = isElementDropDown ? 
                GetElementsText(selector) :
                GetElementsText(selector).Skip(1);
            
            if (elements == null)
            {
                return true;
            }

            var collection1 = elements.ToArray();
            var orderedElements = isReverseOrder
                ? collection1.OrderByDescending(x => x).ToArray()
                : collection1.OrderBy(x => x).ToArray();
            return AreCollectionsEqual(collection1, orderedElements, isComparePrecisely: true);
        }

        /// <summary>
        /// Checks whether element collection is sorted ascendingly or descendingly 
        /// </summary>
        /// <param name="selector">Elements selector</param>
        /// <param name="isDescendingOrder">Should reverse sort order be applied or not (optional, false
        /// by default)</param>
        /// <returns>(<see cref="bool"/>) True if sorted correctly</returns>
        protected bool IsSortedByNumber(string selector, bool isDescendingOrder = false)
        {
            var elements = GetElementsText(selector)?
                .Select(x => long.Parse(
                    string.Join(
                        string.Empty, 
                        x.Trim().Split(new [] {" ", ".", ","}, StringSplitOptions.RemoveEmptyEntries))))
                .ToArray();
            if (elements == null)
            {
                return false;
            }
            var orderedElements = isDescendingOrder
                ? elements.OrderByDescending(x => x).ToArray()
                : elements.OrderBy(x => x).ToArray();

            return elements.SequenceEqual(orderedElements);
        }

        /// <summary>
        /// Checks whether element collection is alphabetically or reverse alphabetically sorted
        /// by specified web element attribute value 
        /// </summary>
        /// <param name="selector">Elements selector</param>
        /// <param name="attribute">Web element attribute name</param>
        /// <param name="isReverseOrder">Should reverse sort order be applied or not</param>
        /// <returns>(<see cref="bool"/>) True if sorted correctly</returns>
        protected bool IsAlphabeticallySortedByAttribute(string selector, string attribute, bool isReverseOrder = false)
        {
            WaitForPageReady(TestConfig.ImplicitWaitTimeout);
            Thread.Sleep(1000);
            var elements = GetElementsAttribute(selector, attribute).ToArray();
            if (elements.Length == 0)
            {
                return true;
            }
            var orderedElements = isReverseOrder
                ? elements.OrderByDescending(x => x).ToArray()
                : elements.OrderBy(x => x).ToArray();
            return AreCollectionsEqual(elements, orderedElements, isComparePrecisely: true);
        }

        /// <summary>
        /// Moves place label elements on child places map a bit right and lower to show their
        /// center and make them draggable for sure
        /// </summary>
        /// <param name="selector">Child places map label's center selector</param>
        protected void MakeMapMarkersAccessible(string selector)
        {
            try
            {
                ((IJavaScriptExecutor) Browser.CurrentDriver)
                    .ExecuteScript(@"
                        const jElements = $('" + selector + @"');
                        if (jElements.length === 0) {
                            return;    
                        }
                        const elements = Array.from(jElements);
                        elements.forEach(
                            function(item) { 
                                item.style.transform = 'translate(10px, 10px)';
                            });"
                    );
            }
            catch (Exception ex)
            {
                throw new Exception($"JS error: {ex.Message}");
            }
        }

        /// <summary>
        /// Checks whether web elements texts/values are sorted in specific order by the sample.
        /// If text or value is not found in the sample, this text or value will be ignored. 
        /// </summary>
        /// <param name="selector">Elements selector</param>
        /// <param name="sortSample">Sample collection whose values and their order will be used
        /// as a sample of sort order</param>
        /// <param name="isReverseOrder">Use reverse sort order (optional)</param>
        /// <returns>(<see cref="bool"/>) True if elements are sorted correctly</returns>
        protected bool IsSortedInSpecificOrder(string selector, IEnumerable<string> sortSample, 
            bool isReverseOrder = false)
        {
            WaitForPageReady(TestConfig.ImplicitWaitTimeout);
            var elements = GetElementsText(selector);
            if (elements == null)
            {
                return true;
            }

            var collection1 = elements.ToArray();
            var orderedElements = isReverseOrder
                ? collection1.OrderByDescending(x => sortSample.Intersect(collection1).ToList().IndexOf(x)).ToList()
                : collection1.OrderBy(x => sortSample.Intersect(collection1).ToList().IndexOf(x)).ToList();
            return AreCollectionsEqual(collection1, orderedElements, isComparePrecisely: true);
        }

        /// <summary>
        /// Gets web elements text (not value or placeholder value) by their selector
        /// </summary>
        /// <param name="selector">Web elements selector</param>
        /// <returns>(<see cref="IEnumerable{String}"/>) Returns text collection</returns>
        protected IEnumerable<string> GetElementsText(string selector)
        {
            var wait = new WebDriverWait(
                Browser.CurrentDriver, 
                TimeSpan.FromSeconds(TestConfig.ImplicitWaitTimeout))
            {
                PollingInterval = PollingInterval
            };
            try
            {
                return wait.Until<IEnumerable<string>>(a =>
                {
                    try
                    {
                        var texts = FindElements(selector)?
                            .Select(x => x.Text)
                            .ToArray();
                        return texts;
                    }
                    catch
                    {
                        return null;
                    }
                });
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Tries to display javascript alert window with specified text
        /// </summary>
        /// <param name="text">Alert text</param>
        protected void ShowAlert(string text)
        {
            try
            {
                CloseAlert();
                var url = Browser.CurrentDriver.Url;
                if (url == TestConfig.BaseUrl
                    || TestConfig.BaseUrl.Contains(url)
                    || url.Contains(TestConfig.LoginUrl)
                    || url.Contains(TestConfig.TenantsUrl))
                {
                    return;
                }
                ((IJavaScriptExecutor)Browser.CurrentDriver).ExecuteScript($@"alert('{text}');");
            }
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Tries to close safely javascript alert (if displayed)
        /// </summary>
        public static void CloseAlert()
        {
            IAlert alert;
            try
            {
                alert = Browser.CurrentDriver.SwitchTo().Alert();
            }
            catch
            {
                return;
            }

            if (alert != null)
            {
                try
                {
                    alert.Accept();
                    return;
                }
                catch
                {
                    // ignored
                }

                try
                {
                    alert.Dismiss();
                }
                catch
                {
                    // ignored
                }
            }
        }

        protected void AcceptAlert()
        {
            Browser.CurrentDriver.SwitchTo().Alert().Accept();
        }

        /// <summary>
        /// Tries to close all modal dialogs (if any)
        /// </summary>
        public static void CloseModalDialogs()
        {
            CloseAlert();
            var sw = new Stopwatch();
            FileManager.CloseGuiWindow();
            sw.Start();
            while (sw.Elapsed < TimeSpan.FromSeconds(4) &&
                   (IsElementFoundQuickly(CommonElement.ModalDialogDefaultButton1, 0.5)
                    || IsElementFoundQuickly(CommonElement.ModalDialogDefaultButton2, 0.5)))
            {
                Click(CommonElement.ModalDialogDefaultButton1, ignoreIfNoElement: true, 0.5);
                Click(CommonElement.ModalDialogDefaultButton2, ignoreIfNoElement: true, 0.5);
            }
            sw.Stop();
        }

        /// <summary>
        /// Verifies whether check box element is turned on
        /// </summary>
        /// <param name="selector">Check box element selector</param>
        /// <returns>(<see cref="bool"/>) True when turned on</returns>
        protected bool IsCheckBoxOn(string selector)
        {
            return IsElementEquals(selector, "On");
        }

        /// <summary>
        /// Verifies whether check box element is turned off
        /// </summary>
        /// <param name="selector">Check box element selector</param>
        /// <returns>(<see cref="bool"/>) True when turned off</returns>
        protected bool IsCheckBoxOff(string selector)
        {
            return IsElementEquals(selector, "Off");
        }

        /// <summary>
        /// Verifies whether check box element is in uninitialized (neutral) state
        /// </summary>
        /// <param name="selector">Check box element selector</param>
        /// <returns>(<see cref="bool"/>) True if not initialized</returns>
        protected bool IsCheckBoxNeutral(string selector)
        {
            return IsElementEquals(selector, string.Empty);
        }

        /// <summary>
        /// Gets CSS parameter value for requested web element
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="cssParamName">CSS parameter of the web element</param>
        /// <returns>(<see cref="string"/>) CSS parameter value</returns>
        protected string GetElementCssValue(string selector, string cssParamName)
        {
            var element = FindElement(selector);
            return element.GetCssValue(cssParamName);
        }

        /// <summary>
        /// Gets requested web element's size as an object
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <returns>(<see cref="Size"/>) Width and height of web element</returns>
        protected Size GetElementSize(string selector)
        {
            var element = FindElement(selector);
            return element.Size;
        }

        /// <summary>
        /// Gets web element attribute value
        /// </summary>
        /// <param name="selector">Element selector</param>
        /// <param name="attrName">Attribute name</param>
        /// <returns>(<see cref="string"/>) Attribute value or null</returns>
        protected string GetElementAttribute(string selector, string attrName)
        {
            var wait = new WebDriverWait(
                Browser.CurrentDriver,
                TimeSpan.FromSeconds(TestConfig.TextValueWaitTimeout))
            {
                PollingInterval = PollingInterval
            };
            var element = FindElement(selector);
            try
            {
                return wait.Until(x =>
                {
                    var result = element.GetAttribute(attrName);
                    return string.IsNullOrEmpty(result) ? null : result;
                });
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Get web elements attribute values
        /// </summary>
        /// <param name="selector">Elements selector</param>
        /// <param name="attrName">Attribute name</param>
        /// <returns>(<see cref="IEnumerable{String}"/>) Attribute values collection</returns>
        protected IEnumerable<string> GetElementsAttribute(string selector, string attrName)
        {
            var wait = new WebDriverWait(
                Browser.CurrentDriver,
                TimeSpan.FromSeconds(TestConfig.ImplicitWaitTimeout))
            {
                PollingInterval = PollingInterval
            };
            WaitForPageReady(TestConfig.ImplicitWaitTimeout);
            try
            {
                return wait.Until(x =>
                {
                    var elements = FindElements(selector);
                    return elements
                        .Select(element => element.GetAttribute(attrName))
                        .ToArray();
                });
            }
            catch
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Returns ID of an entity which page is currently opened in current browser.
        /// Throws exception when timed out.
        /// </summary>
        /// <returns>(<see cref="long"/>) App, item, or place ID</returns>
        protected long GetEntityIdFromUrl()
        {
            WaitForPageReady(TestConfig.ImplicitWaitTimeout);
            var wait = new WebDriverWait(Browser.CurrentDriver, TimeSpan.FromSeconds(TestConfig.ImplicitWaitTimeout));

            try
            {
                long id = 0;
                wait.Until(x =>
                {
                    var url = GetCurrentUrl();      
                    url = url.Contains('?') 
                        ? url.Substring(url.LastIndexOf('/') + 1, url.IndexOf('?') - url.LastIndexOf('/') - 1)
                        : url.Substring(url.LastIndexOf('/') + 1);
                    return !string.IsNullOrEmpty(url) && long.TryParse(url, out id);
                });
                return id;
            }
            catch 
            {
                throw new WebException("Cannot get entity ID from current URL");
            }
        }

        /// <summary>
        /// Returns text content from Windows clipboard. 
        /// Cannot operate with objects like streams, images, or binaries.
        /// </summary>
        /// <returns>(<see cref="string"/>) Clipboard text content</returns>
        protected string GetClipboardContent()
        {
            return AutoIt.AutoItX.ClipGet();
        }

        /// <summary>
        /// Puts text content to Windows clipboard. Unable to operate with objects
        /// like streams, images, or binaries.
        /// </summary>
        /// <param name="text">Text content</param>
        protected void PutContentToClipboard(string text)
        {
            AutoIt.AutoItX.ClipPut(text);
        }
        
        /// <summary>
        /// Pastes Windows clipboard content to current web element. If fact, this
        /// method sends Ctrl + V.
        /// </summary>
        protected void PasteClipboardContent()
        {
            AutoIt.AutoItX.Send("^v");
        }

        /// <summary>
        /// Closes file download panel below browser window
        /// </summary>
        protected void CloseDownloadPanel()
        {
            var currentTab = GetCurrentTabHandle();
            var handles = GetTabHandles();
            AutoIt.AutoItX.Send("^j");
            while (handles.Equals(GetTabHandles()))
            {
            }
            Thread.Sleep(2000);
            CloseTab(GetTabHandles().Last());
            SwitchToTab(currentTab);
            //AutoIt.AutoItX.Send("^w");
        }
    }
}
