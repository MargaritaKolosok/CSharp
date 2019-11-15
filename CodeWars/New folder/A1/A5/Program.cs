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

        public static int FindEvenIndex(int[] arr)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr.Take(i).Sum() == arr.Skip(i + 1).Sum()) { return i; }
            }
            return -1;
        }

        public static string OddOrEven(int[] array)
        {
            return (array.Sum() % 2 == 0) ? "even" : "odd";            
        }

        static void Main(string[] args)
        {
            Console.WriteLine(XO("xxxooXXOo"));

            Console.WriteLine(Divisors(100));
             // 1,2,4,5,10,20,25,50,100

            Console.WriteLine(PrinterError("aaaxbbbbyyhwawiwjjjwwm"));

            int[] oddOrEvenArray = { 1, 2, 3, 4, 5, 6, 7, 8, 99, 3,4,4,5,5,5 };
            Console.WriteLine(OddOrEven(oddOrEvenArray));

            Console.ReadLine();
        }
    }
}
