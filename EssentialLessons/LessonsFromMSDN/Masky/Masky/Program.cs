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
                    result += cc[i];
                }
                else
                {
                    result += '#';
                }
            }
            return result;
        }

        static void Main(string[] args)
        {
            Console.WriteLine(Maskify("1727272727272772727272"));
            Console.ReadKey();
        }
    }
}
