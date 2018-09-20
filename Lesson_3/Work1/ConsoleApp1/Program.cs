using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            /*
             * 
             * Задание 1 
                Имеется 3 переменные типа int x = 10, y = 12, и z = 3; 
                Выполните и рассчитайте результат следующих операций для этих переменных: 
 
		§ x += y - x++ * z; 
		§ z = --x - y * 5; 
		§ y /= x + 5 % z; 
		§ z = x++ + y * 5; 
		§ x = y - x++ * z;

              */

            int x = 10, y = 12, z = 3;

            x += y - x++ * z;

            Console.WriteLine("{0}", x );

            z = --x - y * 5;

            Console.WriteLine("{0}", z);

            y /= x + 5 % z;

            Console.WriteLine("{0}", y);

            z = x++ + y * 5;

            Console.WriteLine("{0}", z);

            x = y - x++ * z;

            Console.WriteLine("{0}", x);

            Console.ReadKey();



        }
    }
}
