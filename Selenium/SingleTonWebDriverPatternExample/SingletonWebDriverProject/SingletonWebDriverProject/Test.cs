using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SingletonWebDriverProject.Pages;
using SingletonWebDriverProject.WebDriver;

namespace SingletonWebDriverProject
{
    public class Test
    {
        [SetUp]
        public void SetUp()
        {
            Driver.StartBrowser();
            LoginPage loginPage = new LoginPage();
            loginPage.Navigate();
        }
        [TestCase("email", "Password")]
        [Test]
        public void Test1(string email, string password)
        {
            LoginPage.Instance.MakeLogin(email, password);
            
            Assert.AreEqual("https://github.com/sessions/two-factor", Driver.GetCurrentURL());
        }
        [TearDown]
        public void TestEnd()
        {
            Driver.StopBrowser();
        }
    }
}
