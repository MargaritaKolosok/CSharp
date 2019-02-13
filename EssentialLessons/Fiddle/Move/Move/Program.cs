using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Move
{  
    class Program
    {
        static void Main(string[] args)
        {
            // Game game = new Game();
            // game.StartGame();

            string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string Level1 = System.IO.Path.Combine(exePath, "../../levels/Level1.txt");


           // Console.WriteLine();
            MapFromFile mapp = new MapFromFile(Level1);
            // mapp.Show();
            //mapp.FileToArray();
            
            
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
