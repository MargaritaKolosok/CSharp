using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program
    {
        public delegate void myDelegate(string str);

        static void Message(string str)
        {
            Console.WriteLine(str);
            Console.ReadKey();
        }
        static void Main(string[] args)
        {
            Message("Hello");
        }
    }
}
