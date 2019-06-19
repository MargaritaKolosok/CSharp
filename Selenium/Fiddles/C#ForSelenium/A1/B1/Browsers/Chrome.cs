using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using B1.Selenium;

namespace B1.Browsers
{
    class Chrome : IWebDriver
    {
        public void FindElement()
        {
            Console.WriteLine("Find element in Chrome");
        }

        public void Click()
        {
            Console.WriteLine("Click the element in Chrome");
        }

        public void SendKeys(string keys)
        {
            Console.WriteLine("Send keys in Chrome" + keys);
        }
                
    }
}
