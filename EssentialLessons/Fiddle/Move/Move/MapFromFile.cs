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
       // public int BONUS_COUNT;
        public static char BARRICADE = 'X';
        public static char WALL = '*';
        public static char BONUS = '$';
        public static char POINT = '*';
        public static char EXIT = 'E';

        static string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        string dataPath = System.IO.Path.Combine(path, "../..//episodes/episode1.txt");

        public MapFromFile()
        {
            
        }
        public void Show()
        {
            foreach (string line in File.ReadLines(dataPath))
            {
                Console.WriteLine(line);
            }
        }
    }
}
