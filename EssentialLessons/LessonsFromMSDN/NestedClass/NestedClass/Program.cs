using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class Container
{   int x;
    public Container()
    {
        x = 10;
    }
    public int X
    {
        get { return x; }
    }
   public class Nested
    {
        private Container parent;

        public Nested(Container parent)
        {
            this.parent = parent;
        }
        public int GetX()
        {
            return parent.X;
        }
    }
}
namespace NestedClass
{
    class Program
    {
        static void Main(string[] args)
        {
            Container c = new Container();
            Container.Nested my = new Container.Nested(c);
            Console.WriteLine(my.GetX());
            Console.ReadKey();
        }
    }
}
