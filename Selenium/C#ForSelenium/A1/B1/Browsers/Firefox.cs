using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B1.Selenium;

namespace B1.Browsers
{
    class Firefox : IWebDriver
    {
        public void FindElement()
        {
            Console.WriteLine("Find element in Firefox");
        }

        public void Click()
        {
            Console.WriteLine("Click the element in Firefox");
        }

        public void SendKeys(string keys)
        {
            Console.WriteLine("Send keys in Firefox" + keys);
        }
    }
}
