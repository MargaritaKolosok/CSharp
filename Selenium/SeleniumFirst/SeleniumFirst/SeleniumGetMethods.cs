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

        // Get value from drop down

        public static string GetDropDownValue(IWebDriver driver, string element, int index, string type)
        {
            if (type == "Id")
            {
                IWebElement el = driver.FindElement(By.Id(element));
                SelectElement select = new SelectElement(el);
                select.SelectByIndex(index);
                return select.ToString();
            }
            if (type == "Name")
            {
                IWebElement el = driver.FindElement(By.Name(element));
                SelectElement select = new SelectElement(el);
                select.SelectByIndex(index);
                return select.ToString();
            }
            if (type == "XPath")
            {
                IWebElement el = driver.FindElement(By.XPath(element));
                SelectElement select = new SelectElement(el);
                select.SelectByIndex(index);
                return select.ToString(); ;
            }
            else
            {
                return String.Empty;
            }

        }
    }
}
