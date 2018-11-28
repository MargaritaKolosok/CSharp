using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Создать класс Vehicle.
В теле класса создайте поля:  координаты и параметры средств передвижения (цена, скорость, год выпуска).
Создайте 3 производных класса Plane, Саг и Ship.
Для класса Plane должна быть определена высота и количество пассажиров.
Для класса Ship — количество пассажиров и порт приписки. 

Написать программу, которая выводит на экран информацию о каждом средстве передвижения.  
 
*/

class Coordinates
{
    double x, y;
    public Coordinates(double x, double y)
    {
        this.x = x;
        this.y = y;
    }
}

class Vehicle
{
    // Coordinates coordinate;
   private double price;
   private double speed;
   private string year;
   

    public double Price
        {
         get { return price; }
         set { price = value; }
        }
    public double Speed
    {
        get { return speed; }
        set { speed = value; }
    }
    public string Year
    {
        get { return year; }
        set { year = value; }
    }
    
    public virtual void ShowInfo()
    {
        Console.WriteLine("{0}, {1}", price, speed);
    }
    public Vehicle(double speed, string year, double price)
    {
        this.speed = speed;
        this.year = year;
        this.price = price;
    }
}

class Plane : Vehicle
{
    double height;
    int passangers;

    public double Height
    {
        set => height = value;
        get => height;
    }

    public int Passangers
    {
        set => passangers = value;
        get => passangers;
    }

    public Plane(double height, int passangers, double speed, string year, double price)
        :base(speed, year, price)
        {
        this.height = height;
        this.passangers = passangers;
        }

    public override void ShowInfo()        
    {
        base.ShowInfo();
        Console.WriteLine("Height is {0}, Passengers are {1}", height, passangers);
    }
}

class Ship
{

}

namespace Task_3_Ship
{
    class Program
    {
        static void Main(string[] args)
        {
            Plane myPlane = new Plane(100000,221,800, "2002", 1000);

            myPlane.ShowInfo();
            Console.ReadKey();
        }
    }
}
