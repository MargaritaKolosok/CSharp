using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingPoint
{
    static class Coordinates
    {
        public static int Top
        {
            get; set;
        }

        public static int Left
        {
            get; set;
        }
    }

    class Point
    {
        char point = '*';

        public void StartMove()
        {
            Console.SetCursorPosition(Coordinates.Left, Coordinates.Top);
            Console.Write(point);

            ConsoleKeyInfo keyPressed = new ConsoleKeyInfo();

            do
            {
                keyPressed = Console.ReadKey();

                switch (keyPressed.Key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            Coordinates.Top--;
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            Coordinates.Top++;
                            break;
                        }
                    case ConsoleKey.LeftArrow:
                        {
                            Coordinates.Left--;
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            Coordinates.Left++;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                if (Coordinates.Top < 0)
                {
                    Coordinates.Top = 0;
                }
                else if (Coordinates.Left < 0)
                {
                    Coordinates.Left = 0;
                }
                else
                {
                    Draw.DrawPoint(point, Coordinates.Top, Coordinates.Left);
                }

            }
            while (keyPressed.Key != ConsoleKey.Escape);
        }
    }
    static class Draw
    {
        public static void DrawPoint(char point, int top, int left)
        {
            Console.Clear();
            Console.SetCursorPosition(Coordinates.Left, Coordinates.Top);
            Console.Write(point);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Point point = new Point();
            point.StartMove();

            Console.ReadKey();
        }

    }
}
