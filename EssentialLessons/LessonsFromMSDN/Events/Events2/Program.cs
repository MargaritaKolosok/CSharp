using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events2
{
    class KeyPressed
    {
        public delegate void MethodContainer();
        public delegate void MethodContainer2(string str);

        public event MethodContainer StopWord;
        public event MethodContainer2 UppercaseWord;

        public void GetWord()
        {
            string str = Convert.ToString(Console.ReadLine());

            if (str == "stop")
            {
                StopWord();
            }
            else
            {
                UppercaseWord(str);
            }
        }
    }
    class S
    {
        public void Handler1()
        {
            Console.WriteLine("Stop word written");
        }
    }
    class U
    {
        public void Handler2(string str)
        {
            Console.WriteLine(str.ToUpper());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            KeyPressed key = new KeyPressed();
            S s = new S();
            U u = new U();

            key.StopWord += s.Handler1;
            key.UppercaseWord += u.Handler2;

            key.GetWord();
            key.GetWord();
            key.GetWord();

            Console.ReadKey();


        }
    }
}
