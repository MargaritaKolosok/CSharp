﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace MAxValueSeq
{
   public class MyExtensions
    {
        public void GetMax(string str)
        {
            int max = Convert.ToInt32(str.Substring(0, 5));
          
            for (int i = 1; i <= str.Length - 5; i++)
            {
                int num1 = Convert.ToInt32(str.Substring(i, 5));
                Console.WriteLine(str.Substring(i, 5));                
                
               if (max < num1)
                  {
                     max = num1;
                  }
            }

            Console.WriteLine("Max sequence is {0}", max);
            
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string str = "7316717653133062491922511967442657443";
            MyExtensions my = new MyExtensions();
            my.GetMax(str);
            
            Console.ReadKey();
        }
    }
}
