using System;
interface IMyIF_A
{
    int Meth(int x);
}
interface IMyIF_B
{
    int Meth(int x);
}
class myClass : IMyIF_A, IMyIF_B
{
    int IMyIF_A.Meth(int x)
    {
       return x+x;
    }
    int IMyIF_B.Meth(int x)
    {
        return x * x;
    }
    public int MethA(int x)
    {
        IMyIF_A ob = this;
        return ob.Meth(x);
    }
    public int MethB(int x)
    {
        IMyIF_B ob = this;
        return ob.Meth(x);
    }

}
namespace TwoInterfaces
{
    class Program
    {
        static void Main(string[] args)
        {
            myClass myNum = new myClass();            
            Console.WriteLine(myNum.MethA(3));
            Console.WriteLine(myNum.MethB(3));
            Console.ReadKey();
        }
    }
}
