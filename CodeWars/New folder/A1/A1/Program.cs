using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1
{
    class MyClass
    {
        private string name;
        public string Name
        {
            set { name = value; }

            get
            {
                if (name == String.Empty || name == null)
                {
                    return "Name is not defined";
                }
                else
                {
                    return name;
                } 
            }
        }
    }

    enum MyEnum : int
    {
        one = 1,
        two = 2
    }
    class Program
    { 
        class Test
        {
            
            struct Test2
            {
                public int z;
            }
        }

        
        public static string CreatePhoneNumber(int[] numbers)
        {
            string number = "";
            for (int i=0; i<numbers.Length; i++)
            {
                number += numbers[i];
            }
            string temp = "";

            temp = '(' + number.Substring(0, 3) + ')' + ' ' + number.Substring(3, 3) + '-' + number.Substring(6, 4);
            return temp;

        }

        public static string CreatePhoneNumber2(int[] numbers)        {

            string h = string.Concat(numbers).ToString();
            return long.Parse(string.Concat(numbers)).ToString("(000)-000-0000");
        }

        static void Main(string[] args)
        {
            int[] num = new int[] { 1,2,3,4,5,6,7,8,9,0};
            Console.WriteLine(CreatePhoneNumber(num));
            Console.WriteLine(CreatePhoneNumber2(num));

            int[,] mas = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 }, { 10, 11, 12 } };
            int rows = mas.GetUpperBound(0) + 1;
            int columns = mas.Length / rows;
            Console.WriteLine( rows + " " + columns );
           
            Console.WriteLine(MyEnum.one);
            Console.WriteLine();

            MyClass instance = new MyClass();
            //instance.Name;
            Console.WriteLine(instance.Name);
            Console.ReadKey();
        }
    }
}
