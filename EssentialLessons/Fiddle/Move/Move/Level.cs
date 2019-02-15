using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Move
{
    class Level
    {
        static string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static string LevelsFolder = System.IO.Path.Combine(exePath, "../../levels/");

        public static string[] Levels;
        public static int LevelsCount;

        public static void GetAllFiles()
        {
            string[] myFiles = Directory.GetFiles(LevelsFolder);
            Levels = new string[myFiles.Length];
            Levels = myFiles;
            LevelsCount = Levels.Length;
        }
      
    }
}
