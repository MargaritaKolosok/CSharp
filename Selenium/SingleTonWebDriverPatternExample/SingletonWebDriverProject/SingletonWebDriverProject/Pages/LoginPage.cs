using System;
using System.Collections.Generic;
using System.Text;
using SingletonWebDriverProject.WebDriver;

namespace SingletonWebDriverProject.Pages
{
    public partial class LoginPage : WebPage<LoginPage>
    {
        private readonly string _url = "https://github.com/login?return_to=%2Fgit";
        public void Navigate() => WrappedDriver.Navigate().GoToUrl(_url);

        public void MakeLogin(string login, string password)
        {
            loginInput.SendKeys(login);
            passwordInput.SendKeys(password);
            signInButton.Click();
        }
    }
}
