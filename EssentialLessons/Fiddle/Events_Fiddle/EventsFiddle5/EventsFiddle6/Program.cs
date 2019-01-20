using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsFiddle6
{
    class KeyEventArgs : EventArgs
    {
        public char ch;
    }
    class EventKey
    {
        public event EventHandler<KeyEventArgs> KeyPress;

        public void OnKeyPress(char key)
        {
            KeyEventArgs k = new KeyEventArgs();

            if (KeyPress!= null)
            {
                k.ch = key;
                KeyPress(this, k);
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            EventKey keyEvent = new EventKey();

            ConsoleKeyInfo key;

            int count = 0;

            keyEvent.KeyPress += (sender, e) => Console.WriteLine("Get info about pressing key {0}", e.ch);
            keyEvent.KeyPress += (sender, e) => count++;

            Console.WriteLine("To finish programm press '.'");

            do
            {
                key = Console.ReadKey();
                keyEvent.OnKeyPress(key.KeyChar);
            }
            while (key.KeyChar != '.');

            Console.WriteLine($"You've pressed {count} keys");
            Console.ReadKey();
        
        }
    }
}
