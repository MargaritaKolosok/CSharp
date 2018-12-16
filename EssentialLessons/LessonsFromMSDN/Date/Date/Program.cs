using System;

namespace Date
{
    class Program
    {
        static class Year
        {
            public static void CheckYear(DateTime year)
            {
                int Year = year.Year;
                bool result = (Year % 4 ==0) ? true: false;
                if (result)
                    Console.WriteLine("God {0} - visokosnij", Year);
                else
                    Console.WriteLine("God {0} - ne visokosnij", Year);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter year");
            DateTime x = DateTime.Parse(Console.ReadLine());
            Year.CheckYear(x);
            Console.ReadKey();
        }
    }
}
