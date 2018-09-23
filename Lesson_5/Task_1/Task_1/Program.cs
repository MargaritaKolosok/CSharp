using System;

namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Задание 1 
                Известно, что у чисел, которые являются степенью двойки, только один бит имеет значение 1. 
                Используя Visual Studio, создайте проект по шаблону Console Application.  
                Напишите программу, которая будет выполнять проверку – является ли указанное число степенью двойки или нет. 
            */

            long num = 0b100000011;

            /*
             * C#Выделить код

int i = 32;
bool one_bit = Math.Log(i, 2) == (int)Math.Log(i, 2) ? true : false; 
bool one_bit = Convert.ToString(i, 2).Count(x => x == '1') == 1; //или так

             
             */


            Console.WriteLine("является ли указанное число степенью двойки или нет? Введите число:");

          
            Console.ReadKey();

        }
    }
}
