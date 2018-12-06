using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class Num
{
    int num;
    public Num(int num)
    {
        this.num = num;
    }
    public Num Pow(int pow)
    {
        num = Convert.ToInt32(Math.Pow(num, pow));
        return new Num(num);
    }
    public void Show()
    {
        Console.WriteLine("Num is equal to {0}", num);
    }
}

namespace PowAndSqrt
{
    class Program
    {
        static void Main(string[] args)
        {
            Num num = new Num(5);
            num.Show();
            num.Pow(3);
            num.Show();
            Console.ReadKey();
        }
    }
}
