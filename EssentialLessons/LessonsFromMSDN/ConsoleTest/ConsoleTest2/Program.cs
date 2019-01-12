using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest2
{
    class Program
    {
        static void Main(string[] args)
        {
            int a = 10, b = 0;
            int result;
            try
            {
                Console.Out.WriteLine("a/b");
                result = a / b;
                Console.Out.WriteLine(result);
            }
            catch (DivideByZeroException exc)
            {
                Console.Error.WriteLine(exc.Message);
            }
            Console.ReadKey();
        }
    }
}
