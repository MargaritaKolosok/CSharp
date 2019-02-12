using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ReadingFromFile
{
    static class F
    {
        public static int CountLines(string filepath)
        {
            int result = 0;

            using (var input = File.OpenText(filepath))
            {
                while (input.ReadLine()!=null)
                {
                    ++result;
                }

                return result;
            }
        }
    }
    class Program
    {       
        static void Main(string[] args)
        {
            string text = System.IO.File.ReadAllText(@"C:\TestFolder\test.txt");
            text.ToCharArray();
            for (int i=0; i<text.Length; i++)
            {
                Console.WriteLine(text[i]);                
            }

            Console.WriteLine("Count:");
            Console.WriteLine(F.CountLines(@"C:\TestFolder\test.txt"));
            Console.Read();
        }
    }
}
