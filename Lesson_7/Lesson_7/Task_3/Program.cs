using System;

namespace Task_3
{
    class Program
    {

        /*
                 * 
                 * Используя Visual Studio, создайте проект по шаблону Console Application.  
    Напишите метод, который будет определять:  
    1) является ли введенное число положительным или отрицательным.  
    2) Является ли оно простым (используйте технику перебора значений).  


    (Простое число – это натуральное число, которое делится на 1 и само на себя. Чтобы определить простое число или нет, следует найти все его целые делители. Если делителей больше 2-х, значит оно не простое.) 
    3) Делится ли на 2, 5, 3, 6, 9 без остатка.


        */

        static string IsSimple(int val)
        {
            string result;

            int divisionsNumber = 0;

            for (int i = 1; i <= Math.Abs(val); i++)
            {
                if ((Math.Abs(val) % i) == 0)
                {

                    divisionsNumber++;
                }
            }

            if (divisionsNumber == 2)
            {

               result = "Simple";

            }
            else
            {
               result = "Not Simple";
            }

            return result;

        }


        static string IsPositive(int val)
        {
            string result;

            if (val > 0)
            {

                result = "Positive ";

            }
            else
            {
                result = "Negative";
            }

                return result;

        }

        static void Main(string[] args)
        {
       
            Console.WriteLine("Enter number for analysis:");
            int num = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine(IsPositive(num));
            Console.WriteLine(IsSimple(num));

            Console.ReadKey();

        }

    }
}