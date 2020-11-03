using System;
using System.Collections.Generic;

namespace SortedList
{
    class Program
    {
        // SortedList<string, string>

        // Sorted by Key
        static void Main(string[] args)
        {
            var sortedList = new SortedList<string, string>();

            sortedList["rita.kolosok@gmail.com"] = "B Rita, Kolosok";
            
            sortedList["rita.kolosok3@gmail.com"] = "A Rita3, Kolosok3";
            sortedList["rita.kolosok2@gmail.com"] = "H Rita2, Kolosok2";
            sortedList.Add("rita.kolosok4@gmail.com", "Margo Kolosok");
            foreach (object item in sortedList)
            {
                Console.WriteLine(item);
            }

            foreach (KeyValuePair<string, string> name in sortedList)
            {
                Console.WriteLine("Value " + name.Value);
                Console.WriteLine("Key " + name.Key);
            }

            Console.WriteLine("Hello World!");
        }
    }
}
