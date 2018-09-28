using System;

namespace Task_1_1
{
    
    class Program
    {
        static int[] CreateArray(int[] array, int arrLength)
        {
            for (int i=0; i<array.Length; i++)
            {
                array[i] = arrLength * i;
            }

            return array;
        }

        static void ArrayAnalysis(int[] array, out int max, out int min)
        {
            max = 0;
            min = 0;
          
            for (int i=0; i<array.Length; i++)
            {
                if (array[i] > max)
                {
                    max = array[i];
                }

                if (array[i] < min)
                {
                   min = array[i];
                }
            }
                                
        }

        static void Main(string[] args)
        {
                                   
            Console.WriteLine("Enter number of array elements:");
            int arrLength = Convert.ToInt32(Console.ReadLine());

            int[] myArray = new int [arrLength];

            myArray = CreateArray(myArray, arrLength);
           ArrayAnalysis(myArray, out int max1, out int min1);

            for (int i=0; i<myArray.Length; i++)
            {
                Console.WriteLine("Array element {0}", myArray[i]);
            }

            Console.WriteLine("Array MAX element {0}, Array MIN element {1}", max1, min1);
            Console.ReadKey();
        }
    }
}
