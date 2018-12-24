using System;

namespace ArrayT1
{   
    class Program
    {
        static int[,] ArrayGenerator(int x, int y)
        {
            int[,] array = new int[x, y];
            Random random = new Random();
            for (int i=0; i<x; i++)
            {
                for (int j=0; j<y; j++)
                {
                    array[i, j] = random.Next(1,100);
                }
            }
            return array;
            
        }

        static void Main(string[] args)
        {
            
            int[,] arr = ArrayGenerator(5, 5);
            for (int i = 0; i < arr.Length; i++)
            {
                for (int j = 0; j < arr.Length; j++)
                {
                    Console.Write(arr[i,j]);
                }
                Console.WriteLine();
            }
            Console.ReadKey();

        }
    }
}
