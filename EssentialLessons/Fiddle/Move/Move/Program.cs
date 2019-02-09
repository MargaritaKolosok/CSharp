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

        public Point(int left, int top, char symb)
        {
            Left = left;
            Top = top;
            PointSymbol = symb;
        }
        public void Draw()
        {
            DrawPoint(PointSymbol);
        }
        void DrawPoint(char point)
        {
            Console.SetCursorPosition(Left, Top);
            Console.Write(point);
        }
        void Clear()
        {
            DrawPoint(' ');
        }
        public void MoveLeft()
        {
            Left--;
        }
        public void MoveRight()
        {
            Left++;
        }
        public void MoveTop()
        {
            Top--;
        }
        public void MoveDown()
        {
            Top++;
        }
    }
    class Map
    {
        int width = 20, height = 20;
        char[,] Walls = new char[20, 20];

        Point point = new Point(10, 10, '*');

        public Map()
        {
            Console.CursorVisible = false;
            GenerateBorder();
            DrawBorder();
            point.Draw();
        }
        void Barricade(char point, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(point);
        }
        void GenerateBorder()
        { 
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || i == width - 1)
                    {
                        Walls[i, j] = '*';                      
                    }
                    else if (j == 0 || j == height - 1)
                    {
                        Walls[i, j] = '*';                      
                    }
                    else
                    {
                        Walls[i, j] = ' ';                      
                    }
                }
            }
            GenerateBarricades();
        }
        void GenerateBarricades()
        {
            Random random = new Random();
            int counter = 0;
            int x, y;
            void Random()
            {
                x = random.Next(0, width);
                y = random.Next(0, width);
            }
            while(counter < 10)
            {
                Random();

                if (Walls[x, y] != '*' && Walls[x, y] != 'X')
                {
                    Walls[x, y] = 'X';
                    counter++;
                }           
            }          
        }
        void DrawBorder()
        {
            for (int i=0; i< width; i++)
            {
                for (int j=0; j<height; j++)
                {
                    Console.Write(Walls[i,j]);
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
