using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stringObject
{
    class Program
    {
        static void Main(string[] args)
        {
            string str1 = "string 1";
            string str2 = "string 1";
            string str3 = "string 3";
            int result1 = string.Compare(str1, str2);
            int result2 = string.Compare(str1, str3);
            Console.WriteLine("String 1 = '{0}, String 2 = '{1}', String 3 = '{2}", str1, str2, str3);
            Console.WriteLine("Compare result for STR1 and STR2 {0}, Compare result for STR1 and STR3 {1}", result1, result2);
                       
            Console.ReadKey();
        }
    }
}
