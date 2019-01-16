using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Fiddle_2
{
    delegate Delegate2 Delegate1();

    delegate int Delegate2(int x);    

    class Program
    {
        static int Method2(int x)
        {
            return x * x;
        }
        static Delegate2 Method1()
        {           
            return new Delegate2(Method2);
        }

        static void Main(string[] args)
        {
            Delegate1 my = new Delegate1(Method1);
            Delegate2 math = my();
            Console.WriteLine(math(5));
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
