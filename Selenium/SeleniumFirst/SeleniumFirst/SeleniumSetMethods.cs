﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumFirst
{
    static class SeleniumSetMethods
    {
        // Enter Text
        public static void EnterText(IWebDriver driver, string element, string value, string type)
        {
            if (type == "Id")
            {
                driver.FindElement(By.Id(element));
            }
            if (type == "LinkText")
            {
                driver.FindElement(By.LinkText(element));
            }
            if (type == "Name")
            {
                driver.FindElement(By.Name(element));
            }

        }

        // Click

        public static void Click(IWebDriver driver, string element, string type)
        {
            if (type == "id")
            {
                driver.FindElement(By.Id(element)).Click();
            }
            if (type == "LinkText")
            {
                driver.FindElement(By.LinkText(element)).Click();
            }
            if (type == "Name")
            {
                driver.FindElement(By.Name(element)).Click();
            }
        }

        // Select Drop-down control

        public static void SelectDropDown(IWebDriver driver, string element, string value, string type)
        {

            if (type == "Id")
            {
                new SelectElement(driver.FindElement(By.Id(element))).SelectByText(value);
            }
            if (type == "LinkText")
            {
                new SelectElement(driver.FindElement(By.LinkText(element))).SelectByText(value);
            }
            if (type == "Name")
            {
                new SelectElement(driver.FindElement(By.Name(element))).SelectByText(value);
            }           

        }

        // Set value from drop down

        public static string SetDropDownValue(IWebDriver driver, string element, int index, string type)
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
                return select.ToString();
            }
            else
            {
                return String.Empty;
            }

        }
    }
}

/*
- SelectElement class - class in the webdriver.support classes Nuget package
- F9 - Set breakpoint 
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * */