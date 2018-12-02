using System;

interface IEven
{
    bool IsEven(int x);
    bool IsOdd(int x);
}

class checkEven : IEven
{
    bool IEven.IsEven(int x)
    {
        if (x % 2 != 0)
            return true;
        return false;
    }
    public bool IsOdd(int x)
    {
        IEven ob = this;
        return !ob.IsEven(x);
    }
}
namespace isEvenInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            checkEven myNum = new checkEven();
            bool result;
            Console.WriteLine("result = myNum.IsOdd(4);");
            result = myNum.IsOdd(4);
            Console.WriteLine(result);
            Console.WriteLine("IEven ob = (IEven)myNum;");
            IEven ob = (IEven)myNum;
            Console.WriteLine("ob.IsEven(4) : {0}, ob.IsOdd(5) {1}", ob.IsEven(4), ob.IsOdd(5));
            Console.ReadKey();
        }
    }
}
