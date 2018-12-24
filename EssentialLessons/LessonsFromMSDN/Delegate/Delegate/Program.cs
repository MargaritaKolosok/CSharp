using System;

namespace Delegate
{
    delegate string ChangeString(string str);

    class Program
    {
        static string UpperCase(string str)
        {           
            return str.ToUpper();
        }

        static string LowerCase(string str)
        {
            return str.ToLower();
        }

        static void Main(string[] args)
        {
            ChangeString StringChanger = UpperCase;
            string test = "Hello world!";
            Console.WriteLine(StringChanger(test));

            StringChanger = LowerCase;
            Console.WriteLine(StringChanger(test));

            StringChanger += UpperCase;
            Console.WriteLine(StringChanger(test));

            Console.ReadKey();

        }
    }
}
