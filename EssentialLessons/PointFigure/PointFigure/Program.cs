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

    public Point[] pointArray = new Point[SideNumber]; // Sidenumber ???

    public void setPointValues()
    {
       
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
    public double PerimeterCalculator()
    {
        double perimeter = 0;
        for (int i=0; i<sideNumber; i++)
        {
            if (i == sideNumber - 1)
            {
             perimeter += LengthSide(pointArray[0], pointArray[sideNumber-1]);
            }
            else
            {
               perimeter += LengthSide(pointArray[i],pointArray[i+1]);
            }
           
        }

        return perimeter;
    }
}
namespace PointFigure
{
 public class Program
    {
        static void Main(string[] args)
        {
           
           int sideNumber;
            Console.Write("Enter number of sides in the Figure:");
            sideNumber = Convert.ToInt32(Console.ReadLine());

            Figure myFigure = new Figure(sideNumber);
            myFigure.setPointValues();

           Console.WriteLine("Perimeter of the Figure sides is equal to {0}", myFigure.PerimeterCalculator());

           
            Console.ReadKey();

        }
    }
}
