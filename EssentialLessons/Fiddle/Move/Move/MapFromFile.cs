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

        public MapFromFile(string level)
        {
            this.level = level;
        }
        public void Show()
        {
            foreach (string line in File.ReadLines(level))
            {
                Console.WriteLine(line);
            }
        }
       public int CountLines()
        {
            int count = 0;

                foreach (string line in File.ReadLines(level))
                {
                    ++count;
                }
            return count;
            
        }
    }
}
