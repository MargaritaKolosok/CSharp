using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Demo demo = new Demo();
            demo.StartBrowser();
            demo.CssDemo();
            demo.CloseBrowser();
        }
    }
}
