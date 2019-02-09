using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{
    static class Bonus
    {
        static int count = 0;
        public static int Count { get; set; }
    }
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
        public void Clear()
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
                    Walls[i, j] = (i == 0 || i == width - 1 || j == 0 || j == height - 1) ? '*' : ' ';                    
                }
            }
            GenerateBarricades('X');
            GenerateBarricades('$');
        }
        void GenerateBarricades(char ch)
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

                if (Walls[x, y] == ' ')
                {
                    Walls[x, y] = ch;
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
        public void StartGame()
        {
            ConsoleKeyInfo keyPressed = new ConsoleKeyInfo();
            Point oldPoint = new Point();

            do
            {
                keyPressed = Console.ReadKey();
                oldPoint = point;

                switch (keyPressed.Key)
                {
                    case ConsoleKey.UpArrow:
                        {
                            point.MoveTop();
                            break;
                        }
                    case ConsoleKey.DownArrow:
                        {
                            point.MoveDown();
                            break;
                        }
                    case ConsoleKey.LeftArrow:
                        {
                            point.MoveLeft();
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            point.MoveRight();
                            break;
                        }
                    default:
                        {
                            break;
                        }                       
                }
                if (IsBarricade())
                {
                    point = oldPoint;
                }
                else
                {
                    oldPoint.Clear();                    
                }                
                point.Draw();
            }
            while (keyPressed.Key != ConsoleKey.Escape);
        }
        bool IsBarricade()
        {
            if (Walls[point.Top, point.Left] == 'X' || Walls[point.Top, point.Left] == '*')
            {
                return true;
            }
            else if (Walls[point.Top, point.Left] == '$')
            {
                Bonus.Count++;
                ShowResult(Bonus.Count);
                return false;
            }
            else
            {
                return false;
            }
        }
        void ShowResult(int result)
        {
            Console.SetCursorPosition(0, 20);
            Console.WriteLine("Result: " + result);
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
            map.StartGame();            
            Console.ReadKey();
        }
    }
}
