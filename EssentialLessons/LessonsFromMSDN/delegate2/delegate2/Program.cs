using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace delegate2
{
    
    class Program
    {
        delegate string StrMethod(string str);

        class DelegateTest
        {
            static string Reverce(string str)
            {
                string text = "";

                for (int i = str.Length - 1; i == 0; i--)
                {
                    text += str[i];
                }
                return text;
            }
        }

        static void Main(string[] args)
        {
           // DelegateTest test = new DelegateTest();
            StrMethod MakeReverce = new StrMethod(Reverce);
        }
    }
}
