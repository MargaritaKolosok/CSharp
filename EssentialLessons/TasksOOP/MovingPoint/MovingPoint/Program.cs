using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingPoint
{
    static class Coordinates
    {
       static int left = 10;
       static int top = 10;

        public static int Top
        {
            get
            {
                return top;
            }
            set
            {
                top = value;
            }
        }

        public static int Left
        {
            get
            {
                return left;
            }
            set
            {
                left = value;
            }
        }

    }
    class Point
    {
        char point = '*';
        
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
            int i = 20;

            Console.SetCursorPosition(Coordinates.Left, Coordinates.Top);
            Console.Write(point);

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
            Coordinates.Left--;
            DrawPoint('*', Coordinates.Left, Coordinates.Top );
                       
        }
        static void RightHandler()
        {            
            Coordinates.Left++;
            DrawPoint('*', Coordinates.Left, Coordinates.Top);
        }
        static void UpHandler()
        {
            Coordinates.Top--;
            DrawPoint('*', Coordinates.Left, Coordinates.Top);
        }
        static void DownHandler()
        {
            Coordinates.Top++;
            DrawPoint('*', Coordinates.Left, Coordinates.Top);
        }
        static void DrawPoint(char point, int top, int left)
        {
            Console.Clear();
            Console.SetCursorPosition(top, left);
            Console.Write(point);
        }

        static void Main(string[] args)
        {
            Point point = new Point();
            point.MoveLeft += LeftHandler;
            point.MoveRight += RightHandler;
            point.MoveUp += UpHandler;
            point.MoveDown += DownHandler;
          
            point.StartMove();

            Console.ReadKey();
        }
       
    }
}
