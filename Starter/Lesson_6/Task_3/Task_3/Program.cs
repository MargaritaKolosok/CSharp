using System;

namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
        {
            int j = 4, i=1; int N=1;

            while (i <=j)
            {
                N *= i;
                i++;
            }

            Console.WriteLine("N! = {0}", N);
            Console.ReadKey();



        }
    }
}
