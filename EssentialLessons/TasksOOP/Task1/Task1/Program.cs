using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime date = new DateTime(2019, 6, 6);
            TimeSpan timeSpan = new TimeSpan();
            DateTime now = DateTime.Now;

            timeSpan = date - now;

            Console.WriteLine(timeSpan.Days);

            Console.ReadKey();
        }
    }
}
