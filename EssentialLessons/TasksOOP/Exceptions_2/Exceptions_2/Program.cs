using System;

namespace Exceptions_2
{
    class Program
    {
        class ExceptA : Exception
        {
            public ExceptA(string str) : base(str)
            {

            }
            public override string ToString()
            {
                return Message;
            }
        }
        class ExceptB : ExceptA
        {
            public ExceptB(string str) : base(str) { }
            public override string ToString()
            {
                return Message;
            }
        }
        static void Main(string[] args)
        {
            for (int i=0; i<3; i++)
            {
                try
                {
                    if (i == 0)
                    {
                        throw new ExceptA("Перехват исключения типа ExceptA");
                    }
                    if (i == 1)
                    {
                        throw new ExceptB("Перехват исключения типа ExceptB");
                    }
                    else
                    {
                        Exception exc = new Exception();
                        throw new Exception("My exc", exc);
                    }
                }
                catch(ExceptB exc)
                {
                    Console.WriteLine(exc);
                }
                catch (ExceptA exc)
                {
                    Console.WriteLine(exc);
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                }
            }
            Console.ReadKey();
        }
    }
}
