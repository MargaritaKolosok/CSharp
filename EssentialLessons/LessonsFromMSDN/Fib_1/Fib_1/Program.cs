using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * 
Напишите программу, в которой реализовать ситуацию, когда при
вызове экземпляра делегата каждый раз генерируется очередное число в
последовательности Фибоначчи

*/

public delegate int Fibonachi();

namespace Fib_1
{
    class Program
    {
        
        static int Fib = 0;
        private static int x = 1;
        static int y = 1;
       

        public static int NumberGenerate()
        {
            Fib = x + y;
            x = y;
            y = Fib;
            
            return Fib;
        }

        static void Main(string[] args)
        {
            Fibonachi fibonachiNumber = NumberGenerate;
            Console.WriteLine(fibonachiNumber());
            Console.WriteLine(fibonachiNumber());
            Console.WriteLine(fibonachiNumber());
            Console.WriteLine(fibonachiNumber());
            Console.WriteLine(fibonachiNumber());
            Console.WriteLine(fibonachiNumber());

            Console.ReadKey();

        }
    }
}
