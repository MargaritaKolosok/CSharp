using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deletagtes
{
    class Program
    {
        public delegate void Message(string str);

        public static void Display(string str)
        {
            System.Console.WriteLine(str);
        }
        static void Main(string[] args)
        {
            Message message = Display;
            Display("Hello world!");
            Console.ReadKey();
            
        }
    }
}
