using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Filtering
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] numbers = { 1,2,34,54,3,2,3,44,6,78,90};

            IEnumerable<int> evens = numbers.Where(i => i%2 ==0 && i>10);
            foreach (int i in evens)
            {
                Console.WriteLine(i);
            }
            Console.ReadKey();
        }
    }
}
