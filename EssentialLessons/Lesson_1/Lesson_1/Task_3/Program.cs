using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_3
{
    class myClass
    {
        private string field = null;

        public string Field
        {
            set
            {
                field = value;
            }

            get
            {
                return field;
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            myClass instance = new myClass();

            Console.WriteLine(instance.Field);
            instance.Field = "Hello";
            Console.WriteLine(instance.Field);


            Console.ReadKey();
        }
    }
}
