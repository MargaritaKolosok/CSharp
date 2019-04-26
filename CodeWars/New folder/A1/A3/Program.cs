using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace A3
{
    //Write simple.camelCase method (camel_case function in PHP, CamelCase in C# or camelCase in Java) for strings.
    //All words must have their first letter capitalized without spaces.

    static class Extention
    {
        public static string CamelCase(this string str)
        {
            TextInfo CultInfo = new CultureInfo("en-US", false).TextInfo;
            str = CultInfo.ToTitleCase(str);
            str = str.Replace(" ", "");
            return str;
        }
    }

    class Program
    {     
        static void Main(string[] args)
        {

        }
    }
}
