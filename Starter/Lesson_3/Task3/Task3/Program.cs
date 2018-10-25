using System;

namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            const double pi = 3.1415926535897932384626433;

            double result = 0, R = 6;

            result = pi * Math.Pow(R,2);

            Console.WriteLine("Radius = {0}", result);
            Console.ReadKey();
            
        }
    }
}
