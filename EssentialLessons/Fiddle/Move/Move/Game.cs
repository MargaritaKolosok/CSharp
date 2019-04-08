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
        static Point PreviousPortal;

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
               
                _point = MapFromFile.FindInList(map.PortalList, point);

                Point P = map.PortalList.Find(x => x.PointSymbol == _point.PointSymbol && x.Top != _point.Top && x.Left != _point.Left);

                  
                //P = MapFromFile.FindOtherPortalInList(map.PortalList, point);
                point.Top = P.Top;
                point.Left = P.Left;

                PreviousPortal = P;
                PreviousPortal.PointSymbol = _point.PointSymbol;
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
                if (map.IsPortal(oldPoint))
                {
                     // PreviousPortal.Draw();
                      //oldPoint.Draw();
                }
                else
                {
                    oldPoint.Clear();
                    PreviousPortal.PointSymbol = ' ';
                }                
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

// Go to portal
// Remember POINT (Symbol and coordinates) of the portal point set at. 
// Next step
// Point gets new coordinates
// Oldpoint that is portal should be portal again