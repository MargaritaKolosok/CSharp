using System;

namespace Example_1
{    class Program
    {
        public static void PrintValues(int[] myArr)
        {
            foreach (int i in myArr)
            {
                Console.Write("\t{0}", i);
            }
            Console.WriteLine();
        }
        public static void PrintValues(Object[] myArr)
        {
            foreach (Object i in myArr)
            {
                Console.Write("\t{0}", i);
            }
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            
            int[] InitialArray = new int[5] { 1,2,3,4,5};
            Object[] ObjectArray = new Object[5] { 6,7,8,9,10};

            PrintValues(InitialArray);
            PrintValues(ObjectArray);
            System.Array.Copy(InitialArray, ObjectArray, 3);
            PrintValues(InitialArray);
            PrintValues(ObjectArray);
            Console.WriteLine(InitialArray.GetLowerBound(0));
           // int[] ReversedArray = new int[5];
            
            Array.Reverse(InitialArray);
           //   ReversedArray = InitialArray;
            Console.WriteLine("Reverse array:");
            PrintValues(InitialArray);
            Console.ReadKey();

        }
    }
}
