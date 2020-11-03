using System;
using System.Collections;

namespace ProffesionalLessonsSandBox
{
    class Program
    {
        static void Main(string[] args)
        {
            // Collection Hashtable

            var email = new Hashtable();

            email["rita.kolosok@gmail.com"] = "Rita, Kolosok";
            email["rita.kolosok2@gmail.com"] = "Rita2, Kolosok2";
            foreach(object obj in email)
            {
                Console.WriteLine(obj);
            }

            foreach(DictionaryEntry entry in email)
            { 
                Console.WriteLine(entry.Key);
            }

            foreach (object obj in email.Values)
            {
                Console.WriteLine(obj);
                Console.WriteLine(obj.GetHashCode());
            }

        }
    }
}
