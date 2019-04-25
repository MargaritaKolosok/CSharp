using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1
{
    class Program
    {
        public static string CreatePhoneNumber(int[] numbers)
        {
            string number = "";
            for (int i=0; i<numbers.Length; i++)
            {
                number += numbers[i];
            }
            string temp = "";

            temp = '(' + number.Substring(0, 3) + ')' + ' ' + number.Substring(3, 3) + '-' + number.Substring(6, 4);
            return temp;

        }

        public static string CreatePhoneNumber2(int[] numbers)        {

            string h = string.Concat(numbers).ToString();
            return long.Parse(string.Concat(numbers)).ToString("(000) 000-0000");
        }

        static void Main(string[] args)
        {
            int[] num = new int[] { 1,2,3,4,5,6,7,8,9,0};
            Console.WriteLine(CreatePhoneNumber(num));
            Console.WriteLine(CreatePhoneNumber2(num));
            Console.ReadKey();
        }
    }
}
