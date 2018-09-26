using System;

namespace Task_2
{
    /*
     * Имеется N клиентов, которым компания производитель должна доставить товар. Сколько существует возможных маршрутов доставки товара, с учетом того, что товар будет доставлять одна машина?  
Используя Visual Studio, создайте проект по шаблону Console Application.  
Напишите программу, которая будет рассчитывать и выводить на экран количество возможных вариантов доставки товара. Для решения задачи, используйте факториал N!, рассчитываемый с помощью рекурсии. Объясните, почему не рекомендуется использовать рекурсию для расчета факториала. Укажите слабые места данного подхода. 

    */
    
    class Program
    {
        
        static void Main(string[] args)
        {
            
            int factorial(int n)
            {
                if (n == 1 || n == 0)
                {
                    return 1;
                }
                else
                {
                    return n * factorial(n - 1);
                }
                
            }

            Console.WriteLine("Enter Number to make factorial:");
            int N = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine(factorial(N)); 

            Console.ReadKey();

        }
    }
}
