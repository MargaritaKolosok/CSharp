using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * 
 *Создать классы Point и Figure.
Класс Point должен содержать два целочисленных поля и одно строковое поле.
Создать три свойства с одним методом доступа get.

Создать пользовательский конструктор, в теле которого проинициализируйте поля значениями
аргументов.

Класс Figure должен содержать конструкторы, которые принимают от 3-х до 5-ти
аргументов типа Point.

Создать два метода: double LengthSide(Point A, Point B), который рассчитывает длину
стороны многоугольника; void PerimeterCalculator(), который рассчитывает периметр
многоугольника.
Написать программу, которая выводит на экран название и периметр многоугольника. 

 * */
class Point
{
    private int x, y;
    string figureName;

    public int X
    {
        get { return x; }
    }
    public int Y
    {
        get { return y; }
    }
    public string FigureName
    {
        get {
            return figureName;
            }
    }
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

class Figure
{
    public Point point1, point2, point3, point4, point5;
    public Figure(Point point1, Point point2, Point point3)
    {

    }
}
namespace PointFigure
{
    class Program
    {
        static void Main(string[] args)
        {
            Point point1 = new Point(1,2);
            
            Console.WriteLine("X is {0}, y is {1}", point1.X, point1.Y);
            Console.ReadKey();
            
        }
    }
}
