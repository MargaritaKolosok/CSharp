using System;

public interface ISeria
{
    int GetNext();
    void Reset();
    void setStart(int x);
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
    public void setStart(int x)
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

            myNum.setStart(100);
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(myNum.GetNext());
            }
            myNum.Reset();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(myNum.GetNext());
            }
            Console.ReadKey();


        }
    }
}
