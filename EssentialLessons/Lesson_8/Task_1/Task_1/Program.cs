using System;
/*
Используя Visual Studio, создайте проект по шаблону Console Application.
Создайте статический класс с методом void Print (string stroka, int color), который выводит на
экран строку заданным цветом. Используя перечисление, создайте набор цветов, доступных
пользователю. Ввод строки и выбор цвета предоставьте пользователю. 
 * */
static class MyClass
{
    enum Colors { Blue, Red, Green, White };

    static public void Print(string str, int color)
    {
        switch (color)
        {
            case (int)Colors.Blue:
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    break;
                }
            case (int)Colors.Red:
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    break;
                }
            case (int)Colors.Green:
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    break;
                }
            case (int)Colors.White:
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                }
            default:
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    break;
                }
        }

        Console.WriteLine(str);
    }
}


namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = "Hello world";
            MyClass.Print(str, 3);
            Console.ReadKey();
        }
    }
}
