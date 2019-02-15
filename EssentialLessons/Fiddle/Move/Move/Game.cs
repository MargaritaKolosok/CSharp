using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{
    class Game
    {
        Level _level = new Level();
       // static int LevelNum = 0;
        static string level;
        static int num = 0;

        string[] Levels;

        public Game()
        {
            Levels = Level.GetAllFiles();
            foreach (string file in Levels)
            {
                Console.WriteLine(file);
            }
            level = Levels[num];
        }
                
        Point point = new Point(10,10, MapFromFile.POINT);
              
        MapFromFile map = new MapFromFile(level);
        
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
                if (map.IsBarricade(point))
                {
                    point = oldPoint;
                }
                else if (Bonus.Count == map.BONUS_COUNT)
                {
                    map.GenerateBarricades(MapFromFile.EXIT, 1);
                    Bonus.Count = 0;                    
                    oldPoint.Clear();
                }
                else
                {
                    oldPoint.Clear();
                }
                point.Draw();
                ShowResult(Bonus.Count);
            }
            while (keyPressed.Key != ConsoleKey.Escape);
        }
        
        void ShowResult(int result)
        {
            Console.SetCursorPosition(0, MapFromFile.width);
            Console.WriteLine("Result: " + result);
        }        
    }
}
