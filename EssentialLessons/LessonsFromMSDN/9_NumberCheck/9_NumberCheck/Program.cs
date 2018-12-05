using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 *
Дано натуральное число. Определить, является ли оно четным, или оканчивается
цифрой 3.
 */
class Number
{
    int num;
    public Number(int x)
    {
        num = x;
    }
    public bool isEven()
    {
        return (num % 2 == 0);
    }
    public bool isEndsOn()
    {

    }
}
namespace _9_NumberCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            Number num1 = new Number(5);
            Number num2 = new Number(6);
            Console.WriteLine("Num 1 is Even? {0}", num1.isEven());
            Console.WriteLine("Num 2 is Even? {0}", num2.isEven());
            Console.ReadKey();
        }
    }
}
