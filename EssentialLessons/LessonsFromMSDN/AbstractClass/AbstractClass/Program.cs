using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
abstract class baseClass
{
   
    public abstract void Method();
    public abstract int X { set; get; }
    public abstract int Y { set; get; }
}
class myClass : baseClass
{
    public override int X { set; get; }
    public override int Y { set; get; }

    public myClass(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
    public override void Method()
    {
        Console.WriteLine(X*Y);
    }
}
namespace AbstractClass
{
    class Program
    {
        static void Main(string[] args)
        {
            myClass test = new myClass(3,4);
            test.Method();
            Console.ReadKey();
        }
    }
}
