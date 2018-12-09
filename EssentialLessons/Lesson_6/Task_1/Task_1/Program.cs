using System;
/*
Создать статический класс Calculator,
с методами для выполнения основных арифметических операций.
Написать программу, которая выводит на экран основные арифметические операции.  */
static class Calculator
{
    public static double Add(double a, double b)
    {
        return a + b;
    }

    public static double Multiply(double a, double b)
    {
        return a * b;
    }
    public static double Sub(double a, double b)
    {
        return a - b;
    }
    public static double Div(double a, double b)
    {
        if (b == 0) { return 0; }
        return a / b;
    }

}
namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {            
            Console.WriteLine(Calculator.Add(4, 5));
            Console.WriteLine(Calculator.Multiply(4, 5));
            Console.WriteLine(Calculator.Multiply(4, 0));
            Console.WriteLine(Calculator.Sub(4, 5));
            Console.WriteLine(Calculator.Div(4, 5));
            Console.WriteLine(Calculator.Div(4, 0));
            Console.ReadKey();
        }
    }
}
