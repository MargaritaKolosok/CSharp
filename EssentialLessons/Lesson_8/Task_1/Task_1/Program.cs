using System;
/*
Используя Visual Studio, создайте проект по шаблону Console Application.
Создайте статический класс с методом void Print (string stroka, int color), который выводит на
экран строку заданным цветом. Используя перечисление, создайте набор цветов, доступных
пользователю. Ввод строки и выбор цвета предоставьте пользователю. 
 * */
static class MyClass
{
    enum colors {Blue, Red, Green, White};
    static public void Print(string str, int color)
    {
        
    }

}


namespace Task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
