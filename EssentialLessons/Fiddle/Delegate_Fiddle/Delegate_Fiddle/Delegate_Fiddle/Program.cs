using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Fiddle
{
    delegate void myDelegate(int x);

    class Program
    {
        static void Main(string[] args)
        {
            myDelegate my = null;

            my = (int x) =>
            {
                x--;
                if (x >= 0)
                {
                    Console.WriteLine(x);
                    my(x);
                }               
                
                
            };

            my(3);
            Console.ReadKey();
        }
    }
}
