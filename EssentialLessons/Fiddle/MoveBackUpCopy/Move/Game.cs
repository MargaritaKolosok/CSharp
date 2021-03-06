﻿using System;
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
                    map.GenerateBarricades(Map.EXIT, 1);
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
            Console.SetCursorPosition(0, Map.width);
            Console.WriteLine("Result: " + result);
        }        
    }
}
