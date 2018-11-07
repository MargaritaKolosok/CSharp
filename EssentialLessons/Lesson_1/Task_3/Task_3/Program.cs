using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CheckNumber
{
    public bool IsSimple(int x)
    {
        if (x <= 1)  return false; 

        for (int i = 2; i <= x/i; i++)
        
            if ((x % i) == 0)  return false; 

           return true; 

        
    }
}

namespace Task_3
{
    
    class Program
    {
        static void Main(string[] args)
        {
          
            CheckNumber myNumber = new CheckNumber();

            for (int i=2; i<10; i++)
            {
                if (myNumber.IsSimple(i))
                {
                    Console.WriteLine("{0} is Simple Number", i);
                }
                else
                {
                    Console.WriteLine("{0} is NOT Simple Number", i);
                }
               
            }

            Console.ReadKey();
        }
    }
}
