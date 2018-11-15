using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_5
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] myArray = new int[10];

            for (int i = 0; i < myArray.Length; i++)
            {
                double sqrtNum;
                sqrtNum = Math.Pow(i,2);
                Console.WriteLine("POW of {0} is {1}", i, sqrtNum);
                
            }
            Console.ReadKey();
        }
    }
}
