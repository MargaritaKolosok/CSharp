using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * 
Создайте класс Printer.
В теле класса создайте метод void Print(string value), который выводит на экран значение
аргумента.
Реализуйте возможность того, чтобы в случае наследования от данного класса других классов, и вызове
соответствующего метода их экземпляра, строки, переданные в качестве аргументов методов,
выводились разными цветами.
Обязательно используйте приведение типов.

 * */
class Printer
{
    public void Print(string value)
    {
        string[] color = new string[]{ "Gray", "Red" , "Blue"};
        Random random = new Random();
        
        string randomColor = color[random.Next(0,2)];

        switch (randomColor)
        {
            case "Gray":
                Console.BackgroundColor = ConsoleColor.Gray;
                break;
            case "Red":
               Console.BackgroundColor= ConsoleColor.Red;
                break;
            case "Blue":
                Console.BackgroundColor = ConsoleColor.Blue;
                break;
            default:
                Console.BackgroundColor = ConsoleColor.Yellow;
                break;

        }       
       
        Console.WriteLine(value);
    }
}
class Copy : Printer
{
    public Copy(string value)
    {
        
    }
}
namespace Task_1_Printer
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
