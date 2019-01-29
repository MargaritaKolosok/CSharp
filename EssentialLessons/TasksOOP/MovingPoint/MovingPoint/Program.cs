using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingPoint
{
    class Canvas
    {
        int width = 50;
        int height = 50;
        Point point;

        public Canvas(Point point)
        {
            this.point = point;
            DrawCanvas();
        }

        void DrawCanvas()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || i == width - 1)
                    {
                        Console.Write('*');
                    }
                    else if (j==0 || j == height - 1)
                    {
                        Console.Write('*');
                    }
                    else
                    {
                        Console.Write(' ');
                    }
                   
                }
                Console.WriteLine();
            }
        }

    }
        
    class Point
    {
        public delegate void MyHandler();

        public event MyHandler MoveLeft;
        public event MyHandler MoveRight;
        public event MyHandler MoveUp;
        public event MyHandler MoveDown;

        public void OnLeft()
        {
            MoveLeft?.Invoke();
        }
        public void OnRight()
        {
            MoveRight?.Invoke();
        }
        public void OnUp()
        {
            MoveUp?.Invoke();
        }
        public void OnDown()
        {
            MoveDown?.Invoke();
        }

        public void StartMove()
        {
            int i = 10;

            ConsoleKeyInfo keyPressed;                        

            while (i>0)
            {
                keyPressed = Console.ReadKey();
                switch (keyPressed.Key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            OnUp();
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            OnDown();
                            break;
                        }
                    case ConsoleKey.LeftArrow:
                        {
                            OnLeft();
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            OnRight();
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                i--;
            }          
                      
        }
    }
    class Program
    {
        static void LeftHandler()
        {
            Console.WriteLine("Left");
        }
        static void RightHandler()
        {
            Console.WriteLine("Right");
        }
        static void UpHandler()
        {
            Console.WriteLine("Up");
        }
        static void DownHandler()
        {
            Console.WriteLine("Down");
        }

        static void Main(string[] args)
        {
            Point point = new Point();
            point.MoveLeft += LeftHandler;
            point.MoveRight += RightHandler;
            point.MoveUp += UpHandler;
            point.MoveDown += DownHandler;

            Canvas canvas = new Canvas();

            point.StartMove();

            Console.ReadKey();
        }
       
    }
}
