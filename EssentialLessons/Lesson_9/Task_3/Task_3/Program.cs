using System;
public delegate int myDelegate(Num[] arr);

public delegate int Num();

namespace Task_3
{   
    class Program
    {
        public static int Random1()
        {
            Random random = new Random();
            return random.Next(1,10);
        }
        public static int Random2()
        {
            Random random = new Random();
            return random.Next(1, 10);
        }
        public static int Random3()
        {
            Random random = new Random();
            return random.Next(1, 10);
        }

        public static int Avarage(int[] arr)
        {
            int sum = 0;
            for (int i=0; i<arr.Length; i++)
            {
                sum += arr[i];
            }
            return sum / arr.Length;
        }

        static void Main(string[] args)
        {
            Num[] num = new Num[] { Random1, Random2, Random3 };
            
            myDelegate my = Avarage(num);


        }
    }
}
