using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
abstract class baseClass
{
    protected int x;
    protected int y;
    public abstract void Method();
    public abstract int X { get; set; }
    public abstract int Y { get; set; }
}
namespace AbstractClass
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
