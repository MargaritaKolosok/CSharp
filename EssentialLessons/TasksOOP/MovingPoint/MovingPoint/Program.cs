using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingPoint 
{
    class Wall 
    {
        public int Left { get; set; }
        public int Top { get; set; }
    }
    class Walls
    {
        Wall[] WallsArray = new Wall[10];

        public Walls()
        {
            CreateWalls();
            Show();
        }
        void CreateWalls()
        {
            Random random = new Random();
            for (int i=0; i<WallsArray.Length;i++)
            {                
                Wall temp = new Wall();
                temp.Left = random.Next(1, 10);
                temp.Top = random.Next(1, 10);
                WallsArray[i] = temp;
            }
        }
        void Show()
        {
            for (int i=0; i<WallsArray.Length;i++)
            {
                Console.SetCursorPosition(WallsArray[i].Left, WallsArray[i].Top);
                Console.Write('X');
            }
        }
    }
    struct Coordinates
    {
        public int Top
        {
            get; set;
        }

        public int Left
        {
            get; set;
        }
    }

    class Point
    {
        char point = '*';

        Coordinates newPoint;
        Coordinates oldPoint;

     //   bool IsBarricade()
     //   {
            
    //    }

        public void StartMove()
        {
            Console.SetCursorPosition(newPoint.Left, newPoint.Top);
            Console.Write(point);

            ConsoleKeyInfo keyPressed = new ConsoleKeyInfo();

            do
            {
                keyPressed = Console.ReadKey();
                oldPoint = newPoint;
                switch (keyPressed.Key)
                {                    
                    case ConsoleKey.UpArrow:
                        {                            
                            newPoint.Top--;                            
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            newPoint.Top++;
                            break;
                        }
                    case ConsoleKey.LeftArrow:
                        {
                            newPoint.Left--;
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            newPoint.Left++;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                if (newPoint.Top < 0)
                {
                    newPoint.Top = 0;
                }
                else if (newPoint.Left < 0)
                {
                    newPoint.Left = 0;
                }
                else
                {
                    Draw.DrawPoint(point, newPoint.Top, newPoint.Left);
                    Draw.DrawPoint(' ', oldPoint.Top, oldPoint.Left);
                }
            }
            while (keyPressed.Key != ConsoleKey.Escape);
        }
    }
    static class Draw
    {
        public static void DrawPoint(char point, int top, int left)
        {            
            Console.SetCursorPosition(left, top);
            Console.Write(point);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Walls wall = new Walls();
            Point point = new Point();
            point.StartMove();            

            Console.ReadKey();
        }
    }
}
