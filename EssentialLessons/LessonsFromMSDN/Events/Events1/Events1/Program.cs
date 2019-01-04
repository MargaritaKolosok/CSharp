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
        
        public void Count()
        {
            for (int i=0; i<100; i++)
            {
                if (i=-50)
                {

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
        }
    }
}
