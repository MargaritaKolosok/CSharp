using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Number
{
    private int x, y;
    
    public Number(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public bool HasComFactor(out int least, out int greatest)
    {
        least = 1;
        greatest = 1;

        bool first = true;

        int max = x > y? x: y;

        for (int i=2; i<max/2+1; i++)
        {
            if (x%i==0 && y%i==0)
            {
                if (first)
                {
                    least = i;
                    first = false;
                }
                else greatest = i;
            }
        }
        if (first != true) return true;
        else return false;
       
       
    }
}

namespace _10_MethodOutRef
{
    class Program
    {
        static void Main(string[] args)

        {
            Number myNumber = new Number(30, 90);
            
           // myNumber.HasComFactor(out int a, out int b);
            Console.WriteLine(myNumber.HasComFactor(out int a, out int b));
            Console.WriteLine("{0}, {1}", a, b);
            Console.ReadKey();
        }
    }
}
