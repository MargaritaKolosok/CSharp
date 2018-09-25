using System;

namespace Task_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
             * 
             * Используя Visual Studio, создайте проект по шаблону Console Application.  
             * 
                Напишите программу, которая будет выполнять конвертирование валют.  
                Пользователь вводит: 
                сумму денег в определенной валюте. 
                курс для конвертации в другую валюту. 
                Организуйте вывод результата операции конвертирования валюты на экран. 
                
             
             */
            decimal sum, currency, result;

            decimal CurrencyConverter(decimal val1, decimal val2)
            {
                return val1 * val2;
            }

            Console.Write("Amount of money = ");
            sum = Convert.ToDecimal(Console.ReadLine());

            Console.Write("Currency exchange = ");
            currency = Convert.ToDecimal(Console.ReadLine());

            result = CurrencyConverter(sum, currency);

            Console.WriteLine("Sum by currency is = {0:C2}", result);
            Console.ReadKey();
        }
    }
}
