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
    class SeleniumSetMethods
    {
        public static void EnterText(string element, string value, PropertiesType elementType)
        {          
            if (elementType == PropertiesType.Id)
            {
                PropertiesCollection.driver.FindElement(By.Id(element)).SendKeys(value); ;
            }
            else if (elementType == PropertiesType.Name)
            {
                PropertiesCollection.driver.FindElement(By.Name(element)).SendKeys(value); ;
            }           
        }
        public static void Click(string element, PropertiesType elementType)
        {
            if (elementType == PropertiesType.Id)
            {
                PropertiesCollection.driver.FindElement(By.Id(element)).Click();
            }
            else if (elementType == PropertiesType.Name)
            {
                PropertiesCollection.driver.FindElement(By.Name(element)).Click(); 
            }
        }
        public static void SelectDropDown(string element, string value, PropertiesType elementType)
        {            
            if (elementType == PropertiesType.Id)
            {
                new SelectElement(PropertiesCollection.driver.FindElement(By.Id(element))).SelectByText(value);
            }
            else if (elementType == PropertiesType.Name)
            {
                new SelectElement(PropertiesCollection.driver.FindElement(By.Name(element))).SelectByText(value);
            }
        }
    }

}
