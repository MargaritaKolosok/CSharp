using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace Move
{
    class MapFromFile
    {
        public int BONUS_COUNT;
        public static int width, height;
        string level;
        public char[,] Walls;
        public IGraficPoint[,] GraficArray;

        public MapFromFile(string level)
        {
            this.level = level;
            CountLines();

            Walls = new char[width, height];       
            GraficArray = new IGraficPoint[width, height];

            ArrayFromFile();
            BONUS_COUNT = CountBonus();
            ConvertToGraficWalls(Walls);
            DrawMap(GraficArray);
        }
       
        private void CountLines()
        {
            int count = 0;
            
            foreach (string line in File.ReadLines(level))
            {
                ++count;
                height = line.Length;
            }

            width = count;            
        }

        private int CountBonus()
        {
            int count = 0;
            foreach (char point in Walls)
            {
                if(point == Grafic.BONUS)
                {
                    count++;
                }
            }
            return count;
        }

        private void ArrayFromFile()
        {
            string[] STRArray = new string[height];
            List<string> STRList = new List<string>();

            foreach (string line in File.ReadLines(level))
            {
                STRList.Add(line);
            }

            STRArray = STRList.ToArray();

            for (int top = 0; top < width; top++)
            {
               string temp = STRArray[top];
               char[] tempArray = temp.ToCharArray();

               for (int left = 0; left < height; left++)
               {
                  Walls[top, left] = tempArray[left];                
               }                
            }
        }

        private void ConvertToGraficWalls(char[,] Walls)
        {
            for (int top = 0; top < width; top++)
            {
                for (int left = 0; left < height; left++)
                {
                    if (Walls[top, left] == Grafic.WALL)
                    {
                        GraficArray[top, left] = new Grafic.GRAFIC_WALL();
                    }
                    else if (Walls[top, left] == Grafic.BARRICADE)
                    {
                        GraficArray[top, left] = new Grafic.GRAFIC_BARRICADE();
                    }
                    else if (Walls[top, left] == Grafic.BONUS)
                    {
                        GraficArray[top, left] = new Grafic.GRAFIC_BONUS();
                    }
                    else
                    {
                        GraficArray[top, left] = new Grafic.GRAFIC_SPACE();
                    }
                }
            }
        }

        private void DrawBarricade(IGraficPoint point, int left, int top)
        {
            Console.SetCursorPosition(left, top);
           
            Console.ForegroundColor = (ConsoleColor)point.FOREGROUND;
            Console.BackgroundColor = (ConsoleColor)point.BACKGROUND;
            Console.Write(point.SYMBOL);
        }
        public bool IsBarricade(Point point)
        {
            if (Walls[point.Top, point.Left] == Grafic.BARRICADE || Walls[point.Top, point.Left] == Grafic.WALL)
            {
                return true;
            }
            else if (Walls[point.Top, point.Left] == Grafic.BONUS)
            {
                Bonus.Count++;
                Walls[point.Top, point.Left] = ' ';
                return false;
            }
            else if (Walls[point.Top, point.Left] == Grafic.EXIT)
            {
                if (Game.LevelNum < Level.LevelsCount - 1)
                {
                    Game.LevelNum++;
                    Game game = new Game();
                    game.StartGame();
                }
                else
                {
                    Game.GameOver();
                }
               
               return false;
            }
            else
            {
                return false;
            }
        }
        public void Clear()
        {
            Console.Clear();
        }

        public void GenerateBarricades(IGraficPoint point, int barricades_count)
        {
            Random random = new Random();
            int counter = 0;
            int top, left;
            void Random()
            {
                left = random.Next(0, width);
                top = random.Next(0, width);
            }
            while (counter < barricades_count)
            {
                Random();

                if (GraficArray[top, left].SYMBOL == '.')
                {
                    GraficArray[top, left] = point;
                    Walls[top, left] = point.SYMBOL;
                    DrawBarricade(point, left, top);
                    counter++;
                }
            }
        }
        private void DrawMap(IGraficPoint[,] Walls)
        {
            for (int top = 0; top < width; top++)
            {
                for (int left=0; left < height; left++)
                {
                    DrawBarricade(Walls[top, left], left, top );
                }
                Console.WriteLine();
            }
        }
    }
}
