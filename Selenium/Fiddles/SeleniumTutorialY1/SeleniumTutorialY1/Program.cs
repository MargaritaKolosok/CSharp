﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;

namespace SeleniumTutorialY1
{
    class Program
    {      
        static void Main(string[] args)
        {
            ChromeDriver d = new ChromeDriver();
            bool result = d.FindElement(By.Id("c")).Displayed;
        }

        [SetUp]
        public void Initialize()
        {
            PropertiesCollection.driver = new ChromeDriver();
            PropertiesCollection.driver.Navigate().GoToUrl("http://www.executeautomation.com/demosite/index.html");
           
            Console.WriteLine("URL opened");
        }

        [Test]
        public void ExecuteTest()
        {
            EAPageObject page = new EAPageObject();
            page.txtInitial.SendKeys("executeautomation");
            page.btnSave.Click();       
        }

        [TearDown]
        public void CleanUp()
        {
            PropertiesCollection.driver.Close();
            Console.WriteLine("Browser closed");
            
        }
    }
}
