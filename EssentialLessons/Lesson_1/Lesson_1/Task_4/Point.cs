using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_4
{
    class Point
    {
        private int x, y;

        public int X
        {
            get
            {
                return x;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
        }

        public Point()
        {
            Console.WriteLine("Default constructor!");
        }
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
