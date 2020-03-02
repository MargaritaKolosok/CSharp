using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using SingletonWebDriverProject.WebDriver;

namespace SingletonWebDriverProject.Pages
{
    public static class TestPage
    {
        public static IWebElement loginInput => Driver.Browser.FindElement(By.Id("login_field"));
        public static IWebElement passwordInput => Driver.Browser.FindElement(By.Id("password"));
        public static  IWebElement signInButton => Driver.Browser.FindElement(By.Name("commit"));
        public static void MakeLogin(string email, string password)
        {
            loginInput.SendKeys(email);
            passwordInput.SendKeys(password);
            signInButton.Click();
        }
    }
}

