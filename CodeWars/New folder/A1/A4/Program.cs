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
        static void Main(string[] args)
        {
            string h = "(((((())))))";
            Console.WriteLine(ValidParentheses(h));
            Console.ReadKey();
        }
    }
}
