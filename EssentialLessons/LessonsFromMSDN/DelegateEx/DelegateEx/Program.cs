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

            Console.WriteLine(math);
            Console.ReadKey();
        }
    }
}
