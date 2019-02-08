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
        int[] Border;
        int[] Walls;
        int width = 80, height = 44;
        public Map()
        {                       
            Console.SetWindowSize(width, height);
            Console.CursorVisible = false;
            DrawBorder();
        }
        void DrawBorder()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || i == width - 1) Console.Write("*");
                    else if (j == 0 || j == height - 1) Console.Write("*");
                    else Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
        void Clear()
        {
            Console.Clear();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Map map = new Map();
            Console.ReadKey();

        }
    }
}
