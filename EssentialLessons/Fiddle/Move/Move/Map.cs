using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{
    class Map
    {
        int width = 20, height = 20;
        char[,] Walls = new char[20, 20];

        int WALL_PERCENTAGE = 50;
        static char BARRICADE = 'X';
        static char WALL = '*';
        static char BONUS = '$';
        static char POINT = '*';

        Point point = new Point(10, 10, POINT);

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
                    Walls[i, j] = (i == 0 || i == width - 1 || j == 0 || j == height - 1) ? POINT : ' ';
                }
            }
            GenerateBarricades(BARRICADE);
            GenerateBarricades(BONUS);
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
            while (counter < ((Walls.Length / 100) * WALL_PERCENTAGE))
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
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Console.Write(Walls[i, j]);
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
            if (Walls[point.Top, point.Left] == BARRICADE || Walls[point.Top, point.Left] == WALL)
            {
                return true;
            }
            else if (Walls[point.Top, point.Left] == BONUS)
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
}
