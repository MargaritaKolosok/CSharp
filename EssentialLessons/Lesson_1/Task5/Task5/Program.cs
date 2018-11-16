using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task5
{
    class Rect
    {
        public int Height;
        public int Width;

        public Rect(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }

        public int Area()
        {
            return this.Width * this.Height;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Rect myRect = new Rect(3,5);
            Console.WriteLine("Width is {0}, Height is {1}, Area is {2}", myRect.Width, myRect.Height, myRect.Area());
            Console.ReadKey();
        }
    }
}
