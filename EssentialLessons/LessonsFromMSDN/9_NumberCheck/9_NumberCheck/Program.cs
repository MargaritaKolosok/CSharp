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
    public bool isEndsOn(int i)
    {
      string numStr = num.ToString();
      int last = Convert.ToInt32(numStr.AsEnumerable().Last().ToString());            
      return (last == i);        
    }
}
namespace _9_NumberCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            Number num1 = new Number(53);
            Number num2 = new Number(6);
            Console.WriteLine("Num 1 is Even? {0}, has 3 in the end {1}", num1.isEven(), num1.isEndsOn(3));
            Console.WriteLine("Num 2 is Even? {0} has 3 in the end {1}", num2.isEven(), num2.isEndsOn(3));
            Console.ReadKey();
        }
    }
}
