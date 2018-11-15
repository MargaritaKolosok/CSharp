using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_4
{
    class Program
    {
        static void Main(string[] args)
        {
            Point PointA = new Point(122,44);
            Console.WriteLine("Point.X = {0}, Point.Y = {1}", PointA.X, PointA.Y);

            Point PointB = new Point();
            Console.WriteLine("Point.X = {0}, Point.Y = {1}", PointB.X, PointB.Y);
            Console.ReadKey();

        }
    }
}
