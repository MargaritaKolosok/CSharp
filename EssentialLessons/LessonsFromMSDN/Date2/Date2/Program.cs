using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Date2
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime now = DateTime.Now;
            TimeSpan daysToNewYear;

            DateTime NewYear = new DateTime(now.Year, 1, 1);

            if (now > NewYear)
            {
                NewYear = new DateTime(now.Year + 1, 1, 1);
                daysToNewYear = NewYear - now;
            }
            else
            {
                daysToNewYear = NewYear - now;
            }
            Console.WriteLine(" daysToNewYear {0}", daysToNewYear.Days);
            Console.ReadKey();
        }
    }
}
