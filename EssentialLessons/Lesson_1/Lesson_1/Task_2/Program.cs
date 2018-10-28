using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    class myClass
    {
        private string field = null;

        public void SetString(string value)
        {
            field = value;
        }

        public string GetString()
        {
            return field;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            myClass instance = new myClass();
            Console.WriteLine(instance.GetString());

            instance.SetString("Hello");
            
            Console.WriteLine(instance.GetString());
            Console.ReadKey();
        }
    }
}
