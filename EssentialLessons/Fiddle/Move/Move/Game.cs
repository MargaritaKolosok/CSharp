using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{
    class Game
    {        
        Point point = new Point(10, 10, Map.POINT);
        Map map = new Map();

        public void StartGame()
        {
            point.Draw();
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
            if (map.Walls[point.Top, point.Left] == Map.BARRICADE || map.Walls[point.Top, point.Left] == Map.WALL)
            {
                return true;
            }
            else if (map.Walls[point.Top, point.Left] == Map.BONUS)
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
    }
}
