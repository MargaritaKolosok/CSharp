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
    private Point point1, point2, point3, point4, point5;
    private double sideLendth;
    
    public Figure(Point point1, Point point2, Point point3)
    {
        this.point1 = point1;
        this.point2 = point2;
        this.point3 = point3;
    }
    public Figure(Point point1, Point point2, Point point3, Point point4)
    {
        this.point1 = point1;
        this.point2 = point2;
        this.point3 = point3;
        this.point3 = point4;
    }
    public Figure(Point point1, Point point2, Point point3, Point point4, Point point5)
    {
        this.point1 = point1;
        this.point2 = point2;
        this.point3 = point3;
        this.point3 = point4;
        this.point3 = point5;
    }
    public double LengthSide(Point A, Point B)
    {
        sideLendth = Math.Sqrt(Math.Pow((point2.X - point1.X), 2) + Math.Pow((point2.Y - point1.Y), 2));
        return sideLendth;
    }
    public void PerimeterCalculator()
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
            Point point2 = new Point(4, 6);
            Point point3 = new Point(2, 5);
            Point point4 = new Point(6, 17);
            Point point5 = new Point(7, 9);

            Console.WriteLine("X is {0}, y is {1}", point1.X, point1.Y);

            Figure Triangle = new Figure(point1, point2, point3);
            
            Console.WriteLine(Triangle.LengthSide(point1, point2)); 
            Console.ReadKey();
            
        }
    }
}
