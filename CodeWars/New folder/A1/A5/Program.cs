using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A5
{
    class Program
    {
        public static bool XO(string input)
        {
            return input.ToLower().Count(i => i == 'x') == input.ToLower().Count(i => i == 'o');
        }
        public static int Divisors(int n)
        {
            return Enumerable
                .Range(1, n)
                .Select(x => x)
                .Count(x => n % x == 0);
        }

        public static string PrinterError(String s)
        {
            s.ToLower();
            return s.Count(x => x > 'm') + "/" + s.Count();
        }

        static void Main(string[] args)
        {
            Console.WriteLine(XO("xxxooXXOo"));
            Console.WriteLine(Divisors(100));
            // 1,2,4,5,10,20,25,50,100
            Console.WriteLine(PrinterError("aaaxbbbbyyhwawiwjjjwwm"));
            Console.ReadLine();
        }
    }
}
