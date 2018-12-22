using System;

namespace Task_1
{
    class Program
    {
        public delegate int PerformCalculation(int a, int b, int c);

        public static int Calculation(int a, int b, int c)
        {            
            return (a + b + c) / 3;
        }

        static void Main(string[] args)
        {
            PerformCalculation handler = Calculation;
            
            Console.WriteLine("Result is {0}", handler(1, 2, 3));
            Console.ReadKey();

        }
    }
}
