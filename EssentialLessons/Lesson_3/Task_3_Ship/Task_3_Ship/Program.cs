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
    Coordinates coordinate;
   private double price;
   private double speed;
   private DateTime year;

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
    public DateTime Year
    {
        get { return Convert.ToDateTime(year); }
        set { year = value; }
    }
    public void ShowInfo()
    {
        Console.WriteLine("{0}, {1}, {2}", year, price, speed);
    }

}

class Plane : Vehicle
{
    double height;
    int passangers;

}

namespace Task_3_Ship
{
    class Program
    {
        static void Main(string[] args)
        {
            Plane myPlane = new Plane();
            myPlane.Price = 100000000;
            myPlane.Speed = 200;
            myPlane.Year = 2002;
            myPlane.ShowInfo();
            Console.ReadKey();

        }
    }
}
