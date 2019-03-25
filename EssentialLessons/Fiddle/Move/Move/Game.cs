using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{
    class Game
    {
        public static int LevelNum = 0;
        private static bool isGameOver = false;

        Point point = new Point(10,10, Grafic.POINT);
              
        MapFromFile map = new MapFromFile(Level.Levels[LevelNum]);
        
        Bonus bonus = new Bonus();        

        public void StartGame()
        {
            point.Draw();
           
            ConsoleKeyInfo keyPressed = new ConsoleKeyInfo();
            Point oldPoint = new Point();

            do
            {
                keyPressed = Console.ReadKey(true);
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
                NewMethod(oldPoint);
                point.Draw();
                ShowResult(Bonus.Count);
            }
            while (keyPressed.Key != ConsoleKey.Escape || isGameOver == true);
        }

        private void NewMethod(Point oldPoint)
        {
            if (map.IsBarricade(point))
            {
                point = oldPoint;
            }
            else if (map.IsPortal(point))
            {                
                Point _point = new Point();
                _point = map.PortalList.Find(x => x.PointSymbol == point.PointSymbol && x.Left != point.Left && x.Top != point.Top);                
                point.Top = _point.Top;
                point.Left = _point.Left;
                oldPoint.Clear();
            }
            else if (Bonus.Count == map.BONUS_COUNT)
            {
                Grafic.GRAFIC_EXIT exit;
                map.GenerateBarricades(exit, 1);
                Bonus.Count = 0;
                oldPoint.Clear();
            }
            else
            {
                oldPoint.Clear();
            }
        }

        private void ShowResult(int result)
        {
            Console.SetCursorPosition(0, MapFromFile.width);
            Console.WriteLine("Result: " + result + " ");
        }
        public static void GameOver()
        {
            Console.WriteLine("Game over!");
            Console.ReadKey();
            isGameOver = true;
        }
    }
}
