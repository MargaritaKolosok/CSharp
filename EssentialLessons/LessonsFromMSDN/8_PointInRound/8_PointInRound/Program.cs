using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * 
Ввести с клавиатуры координаты точки А(x,y) и определить лежит ли
данная точка внутри окружности радиуса R. Центром окружности является
начало координат. Ответ вывести в виде сообщения.
 * */

interface RoundFigure
{
    int X { get; set; }
    int Y { get; set; }
    void Show();
}
class Round : RoundFigure
{
    public int X {get; set;}
    public int Y { get; set; }

    private int radius;

    private int a = 0, b = 0; // Point of the Coordinate start

    public Round(int x, int y, int Radius)
    {
        X = x;
        Y = y;
        radius = Radius;
    }

    private bool isInRound()
    {
        bool result = ((Math.Pow((X-0),2) + Math.Pow((Y-0),2))<=(Math.Pow(radius,2)));
        return result;      
        
    }

    public void Show()
    {
        Console.WriteLine("Point X: {0}, Y: {1}, is in Round: {2}",X, Y, isInRound());
    }
}
namespace _8_PointInRound
{
    class Program
    {
        static void Main(string[] args)
        {
            Round myRound = new Round(5,0,2);
            myRound.Show();
            Console.ReadKey();
        }
    }
}
