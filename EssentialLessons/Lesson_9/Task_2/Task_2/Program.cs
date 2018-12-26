using System;

namespace Task_2
{
    class Program
    {
        public delegate int Calculation(int x, int y);
        public static Calculation Add = delegate (int x, int y) { return x + y; };
        public static Calculation Sub = (int x, int y) => {return x - y;};
        public static Calculation Mul = (x, y) => x* y;
        public static Calculation Div = (x, y) => (y != 0) ? x / y : 0;


        static void Main(string[] args)
        {            
            Console.WriteLine(Add(2, 3));
            Console.WriteLine(Sub(2, 3));
            Console.WriteLine(Mul(11, 3));
            Console.WriteLine(Div(12, 3));
            Console.WriteLine(Div(12, 0));
            Console.ReadKey();
        }
    }
}
