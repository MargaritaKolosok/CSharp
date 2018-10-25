using System;

namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             * Дано два числа A и B (A<B) выведите суму всех чисел расположенных между данными числами на экран. 
                Дано два числа A и B (A<B) выведите все нечетные значения, расположенные между данными числами. 

             */

            int A = 1, B = 20, result_1=0; int i;

            for (i = A+1; i < B; i++) {
                result_1 += i;
                if (i%2!=0) {

                    Console.WriteLine("Нечетное число в диапазоне: {0}", i);
                }
            }
            
            Console.WriteLine("Result_1: {0}", result_1);
            Console.ReadKey();
        }
    }
}
