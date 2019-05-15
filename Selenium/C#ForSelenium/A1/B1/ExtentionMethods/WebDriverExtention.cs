using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B1.Selenium;

namespace B1.Selenium
{
    public static class WebDriverExtention
    {
        public static void SendKeysWithSplChar(this IWebDriver driver, string keys, string splChar)
        {
            driver.SendKeys(keys);
            driver.SendKeys(splChar);
        }
    }
}
