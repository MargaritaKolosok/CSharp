using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumFirst
{
    class SeleniumGetMethods
    {
        public static string GetText(IWebDriver driver, string element, string type)
        {
            if (type == "Id")
            {
                return driver.FindElement(By.Id(element)).Text;
            }
            if (type == "Name")
            {
                return driver.FindElement(By.Name(element)).Text;
            }
            else
            {
                return String.Empty;
            }                
        }
        
    }
}
