using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events1
{ 
    class Counter
    {
        public delegate void MethodContainer();

        public event MethodContainer OnCount;
        
        public void Count()
        {
            for (int i=0; i<100; i++)
            {
                if (i==50)
                {
                    OnCount();
                }
            }
        }
    }
    class Handler_1
    {
        public void Message()
        {
            Console.WriteLine("Handler 1 reaction");
        }
    }
    class Handler_2
    {
        public void Message()
        {
            Console.WriteLine("Handler 2 reaction");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Counter myCount = new Counter();
            Handler_1 handler1 = new Handler_1();
            Handler_2 handler2 = new Handler_2();

            myCount.OnCount += handler1.Message;
            myCount.OnCount += handler2.Message;

            myCount.Count();

            Console.ReadKey();

        }
    }
}
