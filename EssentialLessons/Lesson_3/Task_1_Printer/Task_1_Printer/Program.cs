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
        string[] color = new string[]{ "Green", "Red" , "Blue"};
        Random random = new Random();
        
        string randomColor = color[random.Next(0,3)];

        switch (randomColor)
        {
            case "Green":
                Console.BackgroundColor = ConsoleColor.Green;
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
    public void Print(string value)        
    {
        Console.WriteLine(value);
    }
}
namespace Task_1_Printer
{
    class Program
    {
        static void Main(string[] args)
        {
            Copy myCopy = new Copy();
            // Print Method from Copy class
            myCopy.Print("Value to Print");
            // Print Method from Base Print Class
            Printer anotherCopy = myCopy;
            anotherCopy.Print("Copy from another copy");
                
            Console.ReadKey();

        }
    }
}
