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
        string[] Levels = new string[10];

        public Level()
        {
            Levels = GetAllFiles();
        }
        public string this[int index]
        {
            get { return Levels[index]; }            
        }
      public static string[] GetAllFiles()
        {
            string[] myFiles = Directory.GetFiles(LevelsFolder);
            return myFiles;
        }
    }
}
