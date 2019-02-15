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
        int levelNum = 0;
        List<string> fileEntries= new List<string>();

        public Level()
        {
            GetLevels();
            
            Level.Path = fileEntries[levelNum];
            levelNum++;
        }

        static string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static string LevelsFolder = System.IO.Path.Combine(exePath, "../../levels/");

        void GetLevels()
        {
            string[] files = Directory.GetFiles(LevelsFolder);

            foreach(string file in files)
            {
                fileEntries.Add(file);
            }           
        }

        public static string Path
        {
            get; set;
        }
    }
}
