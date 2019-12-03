using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A2
{
  // Kata.Arithmetic(5, 2, "add")      => 7

    class Program
    {
        public enum Operation
        {
            add =1,
            subtract,
            multiply,
            divide
        }

        public static double Arithmetic(double a, double b, string op)
        {
            double result = 0.0;            

            switch (op)
            {
                case "add":
                    result = a + b;
                    break;
                case "subtract":
                    result = a + b;
                    break;
                case "divide":
                    result = a + b;
                    break;
                case "multiply":
                    result = a + b;
                    break;

                default: throw new NotImplementedException();
            }

            return result;
        }

        static void Main(string[] args)
        {
            // ITVDN



            // ITVDN
            Console.WriteLine(Arithmetic(5,7, "add"));
            Console.ReadKey();
        }
    }
}
