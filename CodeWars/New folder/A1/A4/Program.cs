using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A4
{
    class Program
    {
        public static bool ValidParentheses(string input)
        {
            int left = 0;
            int right = 0;
            int length = input.Length;

            if (length <= 100 && length >= 0)
            {
                foreach (char x in input)
                {
                    if (x == '(')
                    {
                        left++;
                    }
                    else if (x == ')')
                    {
                        right++;
                    }
                }

                return (left == right) ? true : false;
            }
            else
            {
                return false;
            }
            
        }

        public static int RoundToNext5(int n)
        {
            string NumStr = n.ToString();
            if(Char.GetNumericValue(NumStr.ElementAt(NumStr.Length)) > 5)
            {
               int result = Convert.ToInt32(NumStr);
                while(result % 10 == 0)
                {
                    result++;
                }
                return result;
            }
            else
            {
                NumStr.Replace(NumStr.ElementAt(NumStr.Length), '5');
                int result = Convert.ToInt32(NumStr);
                return result;
            }
            
           //return n.ToString().ElementAt(n.ToString().Length)
        }
        static void Main(string[] args)
        {
            string h = "(((((())))))";
            Console.WriteLine(RoundToNext5(134));
            Console.ReadKey();
        }
    }
}
