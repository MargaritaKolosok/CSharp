using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace delegate2
{
    class Str
    {
        public string ReplaceSpaces(string str)
        {
            return str.Replace(' ', '-');
        }
    }
    
    class Program
    {
        public delegate string StrMethod(string str);
       
            static string Reverce(string str)
            {
                string text = "";
                int j = str.Length-1;

            for (int i=0; i< str.Length; i++)
                {
                    text += str[j];
                    j--;
                }
                return text;
            }

        static string UpperCase(string str)
        {
            return str.ToUpper();
        }

        static void Main(string[] args)
        {
           
            StrMethod MakeReverce = new StrMethod(Reverce);
            string text = "Hello world!";

            //text = MakeReverce(text);
            Console.WriteLine(MakeReverce(text));

            MakeReverce = UpperCase;
            Console.WriteLine(MakeReverce(text));
            Console.WriteLine(text);

            MakeReverce = (string str) => { return str.Replace('l', 'L'); };
            Console.WriteLine(MakeReverce(text));

            Str strOb = new Str();

            MakeReverce = strOb.ReplaceSpaces;
            Console.WriteLine(MakeReverce(text));
            Console.ReadKey();
        }

        
    }
}
