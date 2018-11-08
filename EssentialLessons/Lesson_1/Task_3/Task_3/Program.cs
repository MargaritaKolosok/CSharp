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

    public int LeastComFactor(int a, int b)
    {
        int max;

        if (IsSimple(b) || IsSimple(b)) return 1;

        max = (a < b) ? a : b;

        for (int i = 2; i <= max / 2; i++)
            if (((a % i) == 0) && ((b % i) == 0)) return i;
        return 1;
    }
}

namespace Task_3
{
    
    class Program
    {
        static void Main(string[] args)
        {
          
            CheckNumber myNumber = new CheckNumber();
            CheckNumber myNumber2 = new CheckNumber();

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

            int a = 14, b = 49;
            Console.WriteLine("Наименьший общий множитель чисел {0}", myNumber2.LeastComFactor(a, b));

            Console.ReadKey();
        }
    }
}
