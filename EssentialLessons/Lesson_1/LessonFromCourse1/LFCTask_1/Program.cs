using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*Задание 2
Используя Visual Studio, создайте проект по шаблону Console Application.
Требуется: Создать класс с именем Rectangle.

В теле класса создать два поля, описывающие длины сторон double side1, side2.
Создать пользовательский конструктор Rectangle(double side1, double side2), в теле которого
поля side1 и side2 инициализируются значениями аргументов.
Создать два метода, вычисляющие площадь прямоугольника - double AreaCalculator() и периметр
прямоугольника - double PerimeterCalculator().
Создать два свойства double Area и double Perimeter с одним методом доступа get.
Написать программу, которая принимает от пользователя длины двух сторон прямоугольника и выводит
на экран периметр и площадь.
*/
class Rectangle
{
    public double side1;
    public double side2;

    public Rectangle(double side1, double side2)
    {
        this.side1 = side1;
        this.side2 = side2;       
    }

     double AreaCalculator()
    {
        return side1 * side2;
    }

     double PerimeterCalculator()
    {
        return 2*(side1 + side2);
    }

    public double Area
    {
        get { return AreaCalculator(); }
    }

    public double Perimeter
    {
        get { return PerimeterCalculator(); }
    }
}


namespace LFCTask_1
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Side1 = ");
            double side1 = Convert.ToDouble(Console.ReadLine());
            Console.WriteLine("Side2 = ");
            double side2 = Convert.ToDouble(Console.ReadLine());

            Rectangle Square = new Rectangle(side1, side2);
           
           
            Console.WriteLine("Area is {0}, perimeter is {1}", Square.Area, Square.Perimeter);
            Console.ReadKey();
        }
    }
}
