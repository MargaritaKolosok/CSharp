using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agragation
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] numbers = { 1, 2, 3, 4, 10, 34, 55, 66, 77, 88 };

            int size = numbers.Count(i => i%2 ==0 && i > 10);
            int sum = numbers.Sum();

            Console.WriteLine(size);
            Console.WriteLine(sum);
            Console.ReadKey();
        }
    }
}
