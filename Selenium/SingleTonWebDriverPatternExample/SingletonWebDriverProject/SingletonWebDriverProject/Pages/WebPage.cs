using OpenQA.Selenium;
using SingletonWebDriverProject.WebDriver;
using System;
using System.Collections.Generic;
using System.Text;

namespace SingletonWebDriverProject.Pages
{
    public abstract class WebPage<TPage>
    where TPage : new()
    {
        private static readonly Lazy<TPage> _lazyPage = new Lazy<TPage>(() => new TPage());
        protected readonly IWebDriver WrappedDriver;
        protected WebPage() =>
            WrappedDriver = Driver.Browser ?? throw new ArgumentNullException("The wrapped IWebDriver instance is not initialized.");
        public static TPage Instance => _lazyPage.Value;
    }
}
