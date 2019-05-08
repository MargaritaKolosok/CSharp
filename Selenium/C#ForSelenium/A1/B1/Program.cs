using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Console.WriteLine(GetBrowserName(Browser.Firefox));
            Console.ReadKey();
        }
    }
}
