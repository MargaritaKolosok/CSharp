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
            string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string l1 = System.IO.Path.Combine(exePath, "../../levels/Level1.txt");
            Level.Path = l1;
            Game game = new Game();
            game.StartGame();
            
            Console.ReadKey();
        }
    }
}
