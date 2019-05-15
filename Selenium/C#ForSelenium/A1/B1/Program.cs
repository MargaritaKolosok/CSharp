using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B1.Browsers;
using B1.Selenium;

namespace B1
{
    public enum Browser
    {
        Firefox,
        Chrome,
        Opera,
        Explorer
    }

    class Program
    {      
        public static string GetBrowserName(Browser browser)
        {
            if (browser == Browser.Firefox)
            {
                return "firefox";
            }
            if (browser == Browser.Chrome)
            {
                return "chrome";
            }
            if (browser == Browser.Opera)
            {
                return "opera";
            }
            else
            {
                return "explorer";
            }
        }
        static void Main(string[] args)
        {
            WebDriver driver = new Chrome();
            driver.FindElement();

            Console.WriteLine(GetBrowserName(Browser.Firefox));
            Console.ReadKey();
        }
    }
}
