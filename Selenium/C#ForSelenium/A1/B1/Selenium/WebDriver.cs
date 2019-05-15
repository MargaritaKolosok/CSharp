using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B1.Selenium
{
    public class WebDriver
    {
        public void FindElement()
        {
            Console.WriteLine("Finf element");
        }

        public void Click()
        {
            Console.WriteLine("Click the element");
        }

        public void SendKeys(string keys)
        {
            Console.WriteLine("Send keys" + keys);
        }
    }
}
