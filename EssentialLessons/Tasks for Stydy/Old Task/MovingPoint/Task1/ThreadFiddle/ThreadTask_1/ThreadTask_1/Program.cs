using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadTask_1
{
    class Program
    {
        public static void Count()
        {
            for (int i=1; i<9; i++)
            {
                Console.WriteLine("Secondary thread");
                Console.WriteLine(i*i);
                Thread.Sleep(300);
            }
        }

        static void Main(string[] args)
        {
            Thread myThread = new Thread(new ThreadStart(Count));
            myThread.Start();

            for (int i=1; i<9; i++)
            {
                Console.WriteLine("Main Thread");
                Thread.Sleep(400);
            }

            Console.ReadLine();
        }
    }
}
