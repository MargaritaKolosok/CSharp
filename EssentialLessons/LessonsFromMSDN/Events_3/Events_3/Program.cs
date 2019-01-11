using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events_3
{
    public delegate void MyDelegate();

    class Test
    {
        public virtual void Message()
        {
            Console.WriteLine("Test");
        }
    }
    class Test2 : Test
    {
        public override void Message()
        {
            Console.WriteLine("Test2");
        }
    }

    class Program
    {
        public static void Method()
            {

            Console.WriteLine("Hello");

            }


        static void Main(string[] args)
        {
            Test2 my = new Test2();
            MyDelegate test = my.Message;
            test();
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
