using System;

namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
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

            int num;
            string result = "";

            Console.WriteLine("Enter number for analys");
            num = Convert.ToInt32(Console.ReadLine());

            string IsSimple(int val)
            {

                int divisionsNumber = 0;

                for (int i = 1; i == val; i++)
                {


                    Console.WriteLine(val % i);
                    if ((val % i) == 0)
                    {

                        divisionsNumber++;
                    }
                }

                if (divisionsNumber == 2)
                {

                    result += " Simple" + divisionsNumber;

                }
                else
                {
                    result += " Not Simple" + divisionsNumber;
                }

                return result;

            }


            string NumberAnalysis(int val)
            {

                if (num > 0)
                {

                    result += "Positive ";

                }
                else
                {
                    result += "Negative";
                }

                return result;

            }

            Console.WriteLine(NumberAnalysis(num));
              Console.WriteLine( IsSimple(num)); 
            Console.ReadKey();


        }
    }
}