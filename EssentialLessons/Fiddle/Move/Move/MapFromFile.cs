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
        public int BONUS_COUNT = 0;
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
            DrawMap();
        }
        public void Show()
        {
            foreach (string line in File.ReadLines(level))
            {
                Console.WriteLine(line);
            }
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

        void DrawMap()
        {
            for (int i = 0; i < Walls.GetLength(0); i++)
            {
                for (int j=0; j < Walls.GetLength(1); j++)
                {
                    DrawBarricade(Walls[i, j], j, i);
                }
                Console.WriteLine();
            }
        }

        void DrawBarricade(char point, int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(point);
        }


    }
}
