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
        int width = 20, height = 20;
        char[,] Walls = new char[20, 20];

        public Map()
        {
            Console.CursorVisible = false;
            GenerateBorder();
            DrawBorder();           
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
/*    void SetWalls()
        {
            Random random = new Random();
            for (int i = 0; i < 11; i++)
            {
                int x = random.Next(0, 43);
                int y = random.Next(0, 43);
                Walls[x, y] = 'X';
            }
        }

  */


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
