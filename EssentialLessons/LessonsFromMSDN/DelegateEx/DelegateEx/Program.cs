using System;

namespace DelegateEx
{
    delegate double MathDelegate(double x);

    class Program
    {
        static double Double(double x)
        {
            return x * 2;
        }

        static void Main(string[] args)
        {
            MathDelegate math = Double;
            double y = math(3);

            Console.WriteLine(y);

            MathDelegate math2 = delegate (double x) { return x * x; };

            double z = math2(4);
            Console.WriteLine(z);

            MathDelegate math3 = x => x * x * x;

            double c = math3(5);
            Console.WriteLine(c);


            Console.ReadKey();
        }
    }
}
