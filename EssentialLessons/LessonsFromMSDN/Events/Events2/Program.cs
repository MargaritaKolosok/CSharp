﻿using System;
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
                if (StopWord != null)
                {
                    StopWord();
                }
                else
                {
                    Console.WriteLine("StopWord event is null, without subscriptions");
                }
               
            }
            else
            {
                if (UppercaseWord != null)
                {
                    UppercaseWord(str);
                }
                else
                {
                    Console.WriteLine("UpperCaseWord event has no subscriptions to any Methods");
                }               
            }
        }
    }

    static class S
    {
        public static void Handler1()
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
          
            U u = new U();

            key.GetWord();

            key.StopWord += S.Handler1;
            key.UppercaseWord += u.Handler2;

            key.GetWord();
            key.GetWord();
            key.GetWord();

            Console.ReadKey();

        }
    }
}
