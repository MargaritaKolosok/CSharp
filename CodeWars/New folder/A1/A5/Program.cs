using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A5
{
    class Program
    {
        public static bool XO(string input)
        {
            return input.ToLower().Count(i => i == 'x') == input.ToLower().Count(i => i == 'o');
        }

        static void Main(string[] args)
        {
            Console.WriteLine(XO("xxxooXXOo"));
            Console.ReadLine();
        }
    }
}
