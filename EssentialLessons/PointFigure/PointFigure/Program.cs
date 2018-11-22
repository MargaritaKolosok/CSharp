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
  //  string figureName;

    public int X
    {
        get { return x; }
    }
    public int Y
    {
        get { return y; }
    }
    /*    public string FigureName
        {
            get
            {
                return figureName;
            }
        }
    */
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

class Figure
{
    
    private double sideLendth;
    private int sideNumber;

    public Figure(int sideNumber)
    {
        this.sideNumber = sideNumber;
    }
    public int SideNumber
    {
        get { return sideNumber; }
    }
        
    public void setPointValues()
    {
        Point[] pointArray = new Point[sideNumber];

        for (int i = 0; i < sideNumber; i++)
        {
            int x, y;
            Console.WriteLine("Enter X for Point {0}", i);
            x = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter Y for Point {0}", i);
            y = Convert.ToInt32(Console.ReadLine());

            pointArray[i] = new Point(x, y);
        }
       
    }
    
    public double LengthSide(Point A, Point B)
    {
        sideLendth = Math.Sqrt(Math.Pow((B.X - A.X), 2) + Math.Pow((B.Y - A.Y), 2));
        return sideLendth;
    }
    public void PerimeterCalculator()
    {
        double perimeter = 0;
        for (int i=0; i<sideNumber; i++)
        {
            if (i == sideNumber - 1)
            {
                perimeter += LengthSide();
            }
        }

    }
}
namespace PointFigure
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Point point1 = new Point(1, 2);
            Point point2 = new Point(4, 6);
            Point point3 = new Point(2, 5);
            Point point4 = new Point(6, 17);
            Point point5 = new Point(7, 9);

    */

            int sideNumber;
            Console.Write("Enter number of sides in the Figure:");
            sideNumber = Convert.ToInt32(Console.ReadLine());

            Figure Triangle = new Figure(sideNumber);
            Triangle.setPointValues();

            Console.WriteLine(Triangle.SideNumber);

           // Console.WriteLine(Triangle.LengthSide(point1, point2));
            Console.ReadKey();

        }
    }
}
