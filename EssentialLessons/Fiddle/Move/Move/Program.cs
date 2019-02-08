using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{
    struct Point
    {
        public int Left { get; set; }
        public int Top { get; set; }

        public char PointSymbol { get; set; }

        public void Draw()
        {
            DrawPoint(PointSymbol);
        }
        void DrawPoint(char point)
        {
            Console.SetCursorPosition(Left, Top);
            Console.Write(point);
        }
    }
    class Map
    {

        void Clear()
        {
            Console.Clear();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
