using System;

namespace ArrayT1
{   
    static class ExtentionArrayMethod
        {
            public static void ShowArray(this int[,] array)
            {
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        Console.Write(array[i, j]);
                        Console.Write(' ');
                    }
                    Console.WriteLine();
                }
            }

        public static int MaxRowSum(this int[,] array, out int row)
        {
            int max = 0, sum =0;
            row = 0;
            
            for (int i = 0; i < array.GetLength(0); i++)
            {
                sum = 0;

                for (int j = 0; j < array.GetLength(1); j++)
                {
                    sum += array[i, j];
                    
                }

                Console.WriteLine(sum);

                if (max < sum)
                    {
                        max = sum;
                        row = i;
                    }
                }

                return max;
            }
        }
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
            arr.ShowArray();
            Console.WriteLine("Max sum is {0}, in the Row number {1}", arr.MaxRowSum(out int row), row);            
                        
            Console.ReadKey();

        }
    }
}
