using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    class Program
    {
        enum Days : int { Morning = 5, Afternoon = 12, Evening = 15, Night = 23};

        static void Main(string[] args)
        {           

            DateTime date = new DateTime(2019, 6, 6);
            TimeSpan timeSpan = new TimeSpan();
            DateTime now = DateTime.Now;

            int hour = DateTime.Now.Hour;

            if (hour >= (int)Days.Morning && hour <= (int)Days.Afternoon)
            {
                Console.WriteLine("Good morning");
            }
            if (hour >= (int)Days.Afternoon && hour <= (int)Days.Evening)
            {
                Console.WriteLine("Good Afternoon");
            }
            if (hour >= (int)Days.Evening && hour <= (int)Days.Night)
            {
                Console.WriteLine("Good Evening");
            }
            else
            {
                Console.WriteLine("Good Night");
            }
            
            Console.WriteLine();

            Console.ReadKey();
        }
    }
}
