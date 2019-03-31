using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTutorialY1
{
    class SeleniumGetMethods
    {
        public static string GetText(string element, PropertiesType elementType)
        {
            if (elementType == PropertiesType.Id)
            {
                return PropertiesCollection.driver.FindElement(By.Id(element)).GetAttribute("value");
            }
            if (elementType == PropertiesType.Name)
            {
                return PropertiesCollection.driver.FindElement(By.Name(element)).GetAttribute("value");
            }
            else
            {
                return String.Empty;
            }
        }
        public static string GetTextFromDDL(string element, PropertiesType elementType)
        {
            if (elementType == "Id")
            {
                return new SelectElement(PropertiesCollection.driver.FindElement(By.Id(element))).AllSelectedOptions.SingleOrDefault().Text;
            }
            if (elementType == "Name")
            {
                return new SelectElement(PropertiesCollection.driver.FindElement(By.Name(element))).AllSelectedOptions.SingleOrDefault().Text;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}
