using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masky
{
    class Program
    {
        public static string Maskify(string cc)
        {
            string result = "";

            for (int i=0; i<cc.Length; i++)
            {
                if (i < (cc.Length - 4))
                {
                    char ch = (Convert.ToChar(cc[i]) == ' ') ? ' ' : '#';
                    result += ch;
                }
                else
                {
                    result += cc[i];
                }
            }
            return result;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(Maskify("1727 2727 2727 2772"));
            Console.ReadKey();
        }
    }
}
