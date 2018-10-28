using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_1
{
    class myClass
    {
        public string field;
        public void Method()
        {
            Console.WriteLine(field);
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            myClass instance = new myClass();
            instance.field = "Hello World!";

            instance.Method();

            myClass instance2 = new myClass();
            instance2.field = "Was nice to meet you!";
            instance2.Method();

            Console.ReadKey();


        }
    }
}
