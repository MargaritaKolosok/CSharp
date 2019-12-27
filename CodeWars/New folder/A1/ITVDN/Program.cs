using System;

namespace ITVDN
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    Class1 inst1 = new Class1(2, 3);
        //    Class1 inst2 = new Class1(5, 7);

        //    Class1 inst3 = new Class1(inst1.Y, inst2.Y);

        //    Console.WriteLine("X = {0}; Y = {1}", inst3.Y, inst3.Y);

        //    Console.ReadKey();
        //}

        //static Class1 Method(Class1 inst1, Class1 inst2)
        //{
        //    return new Class1(inst1.Y + inst2.Y, inst1.Y + inst2.Y);
        //}

        //class Class1
        //{
        //    int x;
        //    int y;

        //    public int X { set { x = value; } }
        //    public int Y { get { return y; } }

        //    public Class1(int z)
        //    {
        //        x = z;
        //        y = z;
        //    }

        //    public Class1(int x, int y)
        //    {
        //        this.x = x;
        //        this.y = y;
        //    }
        //}
        static void Main(string[] args)
        {
            Class1 inst1 = new Class1(2, 3);
            Class1 inst2 = new Class1(5, 7);

            Class1 inst3 = new Class1(9, 11);

            Console.WriteLine("X = {0}; Y = {1}", inst3.X, inst3.Y);

            Console.ReadKey();
        }

        static Class1 Method(Class1 inst1, Class1 inst2)
        {
            return new Class1(inst1.X + inst2.X, inst1.Y + inst2.Y);
        }

        class Class1
        {
            int x;
            int y;

            public int X { set { x = value; } get { return x; } }
            public int Y { get { return y; } set { y = value; } }

            public Class1(int z)
            {
                x = z;
                y = z;
            }

            public Class1(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}
