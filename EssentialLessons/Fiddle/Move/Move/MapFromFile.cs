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
        //int WALL_PERCENTAGE = 5;
        public int BONUS_COUNT = 10;
        public static char BARRICADE = 'X';
        public static char WALL = '*';
        public static char BONUS = '$';
        public static char POINT = '*';
        public static char EXIT = 'E';

        string level;
        public char[,] Walls;

        int width;
        int height;

        public MapFromFile(string level)
        {
            this.level = level;
            CountLines();
            Walls = new char[width, height];
            ArrayFromFile();
            //DrawMap();
        }
       
      void CountLines()
        {
            int count = 0;
            
            foreach (string line in File.ReadLines(level))
            {
                ++count;
                height = line.Length;
            }

            width = count;            
        }
        
        void ArrayFromFile()
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

        void DrawBarricade(char point, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(point);
        }
        public bool IsBarricade(Point point)
        {
            if (Walls[point.Top, point.Left] == Map.BARRICADE || Walls[point.Top, point.Left] == Map.WALL)
            {
                return true;
            }
            else if (Walls[point.Top, point.Left] == Map.BONUS)
            {
                Bonus.Count++;
                Walls[point.Top, point.Left] = ' ';
                return false;
            }
            else if (Walls[point.Top, point.Left] == Map.EXIT)
            {
                Clear();
                Game game = new Game(level);
                game.StartGame();
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
        public void GenerateBarricades(char ch, int barricades_count)
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

                if (Walls[top, left] == ' ')
                {
                    Walls[top, left] = ch;
                    DrawBarricade(ch, left, top);
                    counter++;
                }
            }
        }


    }
}
