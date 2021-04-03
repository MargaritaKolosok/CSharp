using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeWars
{
    [TestFixture]
    public class DrawFigures
    {
        [TestCase(5)]
        [TestCase(3)]
        [TestCase(12)]
        public void DrawRectangle(int n)
        {
            string[] array = new string[n];

            for (int i = 0; i <n; i++)
            {
                if (i <= n / 2)
                {
                    array[i] = string.Concat(
                    new string(' ', n - i - 1),
                    new string('*', i * 2 + 1),
                    new string(' ', n - i - 1));
                }
                else
                {
                    array[i] = string.Concat(
                      new string(' ', i + 1),
                      new string('*', (n-i)*2-1),
                      new string(' ', i + 1));
                }
                Console.WriteLine(array[i].ToString());
            }
        }

        [TestCase("Hello", ExpectedResult = "72olle")]
        public string ASCIString(string str)
        {
            string result="";

            var last = str[str.Count()-1];

            var ch = Encoding.ASCII.GetBytes(str, 0, 1);
           // str.Replace(str[0],)
            str.Replace(str[str.Count()-1], str[1]);
            str.Replace(str[1], last);
            str.Remove(0,1);
            //str.Substring()
            //var ch = str.ToCharArray();
            //var l = ch[0];
            //var asciChar = Encoding.ASCII.GetBytes(str);
            //var temp = ch[ch.Length-1];
            //ch[ch.Length - 1] = ch[1];
            //ch[1] = temp;

            result = string.Concat(ch, str );
            return result;
        }

        [TestCase("1.2.3","1.0")]
        [TestCase("1.2.3", "1.2.3")]
        [TestCase("1.2.3", "4.0.9")]
        public void CompareVersion(string str1, string str2)
        {
           var result = str1.CompareTo(str2);
            Console.WriteLine(result);
        }
    }

    
}
