using NUnit.Framework;
using System;
using System.Collections.Generic;
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
    }
}
