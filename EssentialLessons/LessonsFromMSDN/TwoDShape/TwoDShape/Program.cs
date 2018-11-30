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
    public void ShowDimmensions()
    {
        Console.WriteLine("Width {0}, Height {1}, Area: {2}", Width, Height, this.Area());
    }
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
class Parallellogram : TwoDShape
{
    private string name;
    public Parallellogram(double w, double h, string name) : base(w, h)
    {
        this.name = name;
    }
    public override double Area()
    {
        return Width * Height;
    }

}
namespace TwoDShapeN
{
    class Program
    {
        static void Main(string[] args)
        {
            Triangle myTriangle = new Triangle(8.0,7.5, "Triangle" );
            
            Console.WriteLine("Triangle Area is {0}", myTriangle.Area());
            myTriangle.ShowDimmensions();

            Parallellogram myPar = new Parallellogram(9.7,2.3,"Parallellogram figure");
            myPar.ShowDimmensions();

            Console.ReadKey();
        }
    }
}
