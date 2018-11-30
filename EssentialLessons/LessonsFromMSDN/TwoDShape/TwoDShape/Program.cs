using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class TwoDShape
{
    private double width, height;
    public double Width
    {
        set => width = value;
        get => width;
    }
    public double Height
    {
        set => height = value;
        get => height;
        
    }
    public TwoDShape(double w, double h)
    {
        Width = w;
        Height = h;

    }
    public abstract double Area();
}
class Triangle : TwoDShape
{
    private string name;
    public Triangle(double w, double h, string s)
        :base(w, h)
    {
        name = s;
    }
    public override double Area()
    {
        return Width * Height / 2;
    }
}
namespace TwoDShapeN
{
    class Program
    {
        static void Main(string[] args)
        {
            Triangle myTriangle = new Triangle(8.0,7.5, "Triangle" );
            double area = myTriangle.Area();
            Console.WriteLine("Triangle Area is {0}, Width is {1}, Height is {2}", area, myTriangle.Width,myTriangle.Height);
            Console.ReadKey();
        }
    }
}
