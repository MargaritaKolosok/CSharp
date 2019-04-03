using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using NUnit.Framework;
using OpenQA.Selenium.Support.PageObjects;

namespace SeleniumTutorialY1
{
    class EAPageObject
    {
        [FindsBy(How = How.Id, Using = "TitleId")]
        public IWebElement TitleId { get; set; }

        [FindsBy(How = How.Name, Using = "Initial")]
        public IWebElement txtInitial { get; set; }
    }
}
