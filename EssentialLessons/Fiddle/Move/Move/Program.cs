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
            Map map = new Map();            
            map.StartGame();            
            Console.ReadKey();
        }
    }
}
