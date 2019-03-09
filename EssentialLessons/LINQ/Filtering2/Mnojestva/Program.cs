using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mnojestva
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] soft = { "Microsoft", "Google", "Apple" };
            string[] hard = { "Apple", "IBM", "Samsung" };

            var result = soft.Except(hard);

            var result2 = soft.Intersect(hard);

            var resultUnion = soft.Union(hard);
            Console.WriteLine();
            Console.WriteLine("Except");

            foreach (string s in result)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine();
            Console.WriteLine("Intersect");

            foreach (string s in result2)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine();
            Console.WriteLine("Union");

            foreach (string s in resultUnion)
            {
                Console.WriteLine(s);
            }

            var resultConcat = soft.Concat(hard);
            Console.WriteLine();
            Console.WriteLine("Concat:");

            foreach (string s in resultConcat)
            {
                Console.WriteLine(s);
            }

            var resultDistinct = resultConcat.Distinct();
            Console.WriteLine();
            Console.WriteLine("Distinct:");

            foreach (string s in resultDistinct)
            {
                Console.WriteLine(s);
            }

            Console.ReadKey();
        }
    }
}
