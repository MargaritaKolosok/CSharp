using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Params
{
    public delegate int myDelegate(params int[] array);

    class Program
    {
        static int Sum(params int[] array)
        {
            int sum = 0;

            for (int i=0; i<array.Length; i++)
            {
                sum += array[i];
            }
            return sum;
        }

        static void Main(string[] args)
        {
            myDelegate Summa = new myDelegate(Sum);
            int[] array = { 1,2,3,4,5,6,7,8,4};
            Console.WriteLine(Summa(array));

            Console.ReadKey();
            
        }
    }
}
