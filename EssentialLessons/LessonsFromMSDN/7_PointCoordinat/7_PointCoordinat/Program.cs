using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
Ввести с клавиатуры координаты точки А(x,y). Определить, в какой четверти
лежит данная точка. Ответ вывести в виде сообщения. 
 
 */
interface Quarter
{
    int X { get; set; }
    int Y { get; set; }
    void Show();
}
class Coordinate: Quarter
{
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }
    private int Quarter()
    {
        int result=0;

        if (X < 0)
        {
            if (Y < 0) result = 3;            
            if (Y > 0) result = 2;
        }

        else if (X > 0)
        {
            if (Y > 0) result = 1;            
            if(Y < 0) result = 4;            
        }

        return result;
    }
    public void Show()
    {
      Console.WriteLine("Point has coordinates: X: {0}, Y: {1}, and places in Quarter {2}", X, Y, Quarter());
    }

}
namespace _7_PointCoordinat
{
    class Program
    {
        static void Main(string[] args)
        {
            Coordinate myCoordinate = new Coordinate(-1, 4);
            myCoordinate.Show();
            myCoordinate.X = 5;
            myCoordinate.Show();

            myCoordinate.Y = -7;
            myCoordinate.Show();
            myCoordinate.X = -3;
            myCoordinate.Show();

            myCoordinate.X = 0;
            myCoordinate.Y = 0;
            myCoordinate.Show();
            Console.ReadKey();
        }
    }
}
