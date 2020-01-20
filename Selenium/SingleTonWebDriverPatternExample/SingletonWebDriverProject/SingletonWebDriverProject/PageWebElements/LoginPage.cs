using System;
using System.Collections.Generic;
using System.Text;
using SingletonWebDriverProject.Pages;
using OpenQA.Selenium;
using SingletonWebDriverProject.WebDriver;

namespace SingletonWebDriverProject.Pages
{
    public partial class LoginPage : WebPage<LoginPage>
    {
        public IWebElement loginInput => Driver.Browser.FindElement(By.Id("login_field"));
        public IWebElement passwordInput => Driver.Browser.FindElement(By.Id("password"));
        public IWebElement signInButton => Driver.Browser.FindElement(By.Name("commit"));
    }
}
