using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleKeyInfo keypress;
            Console.WriteLine("Press some key");

            do
            {
                keypress = Console.ReadKey();
                Console.WriteLine($"Key {keypress} was clicked");

                if ((ConsoleModifiers.Alt & keypress.Modifiers) !=0)
                {
                    Console.WriteLine("Alt key was pressed");
                }
                if ((ConsoleModifiers.Shift & keypress.Modifiers)!=0)
                {
                    Console.WriteLine("Shift key was pressed");
                }
                if ((ConsoleModifiers.Control & keypress.Modifiers) != 0)
                {
                    Console.WriteLine("CTRL was pressed");
                }
            }
            while (keypress.KeyChar != 'Q');
            
                
        }
    }
}
