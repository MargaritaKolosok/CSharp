using System;
/*
Создать статический класс Calculator,
с методами для выполнения основных арифметических операций.
Написать программу, которая выводит на экран основные арифметические операции.  */
static class Calculator
{
    public static int Add(int a, int b)
    {
        return a + b;
    }  
}
namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {            
            Console.WriteLine(Calculator.Add(4, 5));
            Console.ReadKey();
        }
    }
}
