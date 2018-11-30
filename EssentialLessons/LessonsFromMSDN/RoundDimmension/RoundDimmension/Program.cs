using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

abstract class TwoDRound
{
    protected const double Pi = 3.14;
   
    public double Radius
    {
        set;
        get; 
    }
    public double Width
    {
        get;
        set;
    }
    public double Height
    {
        get;
        set;
    }
    public TwoDRound(double radius)
    {
        this.Radius = radius;
    }
    public TwoDRound(double width, double height)
    {
        this.Width = width;
        this.Height = height;
    }
    public abstract double Area();
    public virtual void ShowDimmensions()
    {
        Console.WriteLine("Area is equal to {0}", this.Area());
    }
}
class Round : TwoDRound
{
    public Round(double radius) : base(radius)
    {

    }
  
    public override double Area()
    {
        return Pi * Radius;
    }
    public override void ShowDimmensions()
    {
        base.ShowDimmensions();
        Console.WriteLine("Radis is Equal to {0}", Radius);
    }
}

class Elips : TwoDRound
{
    public Elips(double width, double height) : base(width, height)
    {

    }
    public override double Area()
    {
        return Pi * Width * Height;
    }
    public override void ShowDimmensions()
    {
        base.ShowDimmensions();
        Console.WriteLine("Width is equal to {0}, Height is equal to {1}", Width, Height);
    }
}
    
   
namespace RoundDimmension
{
    class Program
    {
        static void Main(string[] args)
        {
            Round myRound = new Round(5);
            Elips myElips = new Elips(4,5);
            myElips.ShowDimmensions();
            myRound.ShowDimmensions();
            Console.ReadKey();

        }
    }
}
