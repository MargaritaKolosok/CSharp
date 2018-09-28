using System;

namespace Task_1_1
{
    
    class Program
    {
        static int[] ModifyArray(int[] array, int arrLength)
        {
            for (int i=0; i<array.Length; i++)
            {
                array[i] = arrLength * i;
            }

            return array;
        }

        static int ArrayAnalysis(int[] array, out int max)
        {
            max = 0;
            for (int i=0; i<array.Length; i++)
            {
                if (array[i]>max)
                {
                    max = array[i];
                }
            }
            return max;
        }

        static void Main(string[] args)
        {
                                   
            Console.WriteLine("Enter number of array elements:");
            int arrLength = Convert.ToInt32(Console.ReadLine());

            int[] myArray = new int [arrLength];

            myArray = ModifyArray(myArray, arrLength);
            int max = ArrayAnalysis(myArray, out int max1);

            for (int i=0; i<myArray.Length; i++)
            {
                Console.WriteLine("Array element {0}", myArray[i]);
            }

            Console.WriteLine("Array MAX element {0}", max);
            Console.ReadKey();
        }
    }
}
