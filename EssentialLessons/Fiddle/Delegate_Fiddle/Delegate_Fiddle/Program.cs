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

            MathPow anonimMethod1 = new MathPow(delegate(int x) { return (int)Math.Pow(x,4); }); // Anonimus

            MathPow anonim2 = delegate (int x)
            {
                if (x > 0)
                {
                    return x * x;
                }
                else
                {
                    return 0;
                }
            };

            MathPow anonim3 = (int x) => { return x * 2; };

            Console.WriteLine(anonimMethod1(4));

            Console.WriteLine(anonim2(10));
            Console.WriteLine(anonim2(-10));

            Console.WriteLine(anonim3(3));

            Console.WriteLine();
            Console.ReadKey();
           
        }
    }
}
