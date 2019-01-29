using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovingPoint
{
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
            ConsoleKeyInfo keyPressed;
           
            keyPressed = Console.ReadKey();

            Console.WriteLine("Key pressed" + keyPressed);           
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

            point.OnDown();
            point.OnUp();
            point.OnRight();
            point.OnLeft();

            point.StartMove();

            Console.ReadKey();
        }
       
    }
}
