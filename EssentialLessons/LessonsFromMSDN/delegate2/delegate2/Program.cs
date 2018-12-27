using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace delegate2
{
    
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
       

        static void Main(string[] args)
        {
           
            StrMethod MakeReverce = new StrMethod(Reverce);
            string text = "Hello world!";

            Console.WriteLine(MakeReverce(text));
            Console.ReadKey();
        }

        
    }
}
