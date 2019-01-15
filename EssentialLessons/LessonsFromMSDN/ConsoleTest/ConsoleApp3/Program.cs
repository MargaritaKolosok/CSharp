using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream fin;
            string s;

            try
            {
                fin = new FileStream("test.txt", FileMode.Open);
            }
            catch (IOException exc)
            {
                Console.WriteLine(exc.Message);
                return;
            }
            StreamReader fstr_in = new StreamReader(fin);

            try
            {
                while ((s = fstr_in.ReadLine()) != null)
                {
                    Console.WriteLine(s);
                }
            }
            catch (IOException exc)
            {
                Console.WriteLine(exc.Message);
            }
            finally
            {
                fstr_in.Close();
            }
            Console.ReadKey();
        }
    }
}
