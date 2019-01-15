using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegate_Fiddle
{
    class Program
    {
        public delegate int MathPow(int x);

        private static int Pow2(int x)
        {
            Console.WriteLine("pow2");
            return x * x;
            
        }
        private static int Pow3(int x)
        {
            Console.WriteLine("pow3");
            return x * x * x;
        }

        static void Main(string[] args)
        {
            MathPow pow2 = Pow2;
            MathPow pow3 = Pow3;
            Console.WriteLine(pow2(2));
            Console.WriteLine(pow3(2));

            pow2 += Pow3;
            Console.WriteLine(pow2(2));

            Console.ReadKey();
           
        }
    }
}
