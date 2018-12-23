using System;

namespace Task_3
{
    

    class Program
    {
        public delegate double myDelegate(Num[] arr);
        public delegate int Num();

        static Random random = new Random();

        public static int GetRandom()
        {            
            return random.Next(1,100);
        }

        static void Main(string[] args)
        {
            Num[] arr = new Num[10];

            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = () => new Num(GetRandom).Invoke(); 
            }

            myDelegate del = delegate (Num[] c)
            {
                double sum = 0;
                for (int i = 0; i < c.Length; i++)
                {
                    sum += c[i].Invoke();
                }

                return sum / c.Length;
            };

            for (int i = 0; i < arr.Length; i++)
            {
                Console.WriteLine(arr[i].Invoke());
            }

            Console.WriteLine("Avarage is {0}", del(arr));
            Console.ReadKey();

            

        }
    }
}
