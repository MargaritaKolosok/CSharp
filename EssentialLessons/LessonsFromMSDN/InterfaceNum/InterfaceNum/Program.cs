using System;

public interface ISeria
{
    int GetNext();
    void Reset();
    void SetStart(int x);
}

class TwoD : ISeria
{
    int start, num;
    public TwoD()
    {
        start = 0;
        num = 0;
    }
    public int GetNext()
    {
        num += 2;
        return num;
    }
    public void SetStart(int x)
    {
        num = x;
    }
    public void Reset()
    {
        num = start;
    }
}

namespace InterfaceNum
{
    class Program
    {
        static void Main(string[] args)
        {
            TwoD myNum = new TwoD();
            for (int i=0; i<10; i++)
            {
                Console.WriteLine(myNum.GetNext());
            }

            myNum.SetStart(100);

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(myNum.GetNext());
            }

            myNum.Reset();

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(myNum.GetNext());
            }
            ISeria obj = myNum;
            Console.WriteLine(obj.GetNext());
            Console.ReadKey();
        }
    }
}
