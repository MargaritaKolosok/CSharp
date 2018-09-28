using System;

namespace Task_1_1
{
    
    class Program
    {
        static int[] CreateArray(int[] array, int arrLength)
        {
            for (int i=0; i<array.Length; i++)
            {
                array[i] = arrLength * (i+1) + 2*i;
            }

            return array;
        }

        static void ArrayAnalysis(int[] array, out int max, out int min, out int avarage, out int SumEl)
        {
            max = 0;
            min = array[1];
            avarage = 0;
            int Sum = 0;
          
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

                Sum += array[i];
            }

            SumEl = Sum;
            avarage = Sum / array.Length;
                                
        }

        static void Main(string[] args)
        {
                                   
            Console.WriteLine("Enter number of array elements:");

            // User enters Number of Array elements

            int arrLength = Convert.ToInt32(Console.ReadLine());

            // Create myArray

            int[] myArray = new int [arrLength];
            myArray = CreateArray(myArray, arrLength);

            // Check min, max, avarage, Sum numbers in the array

           ArrayAnalysis(myArray, out int maxValue, out int minValue, out int avarageValue, out int SumValue);

            // Show Array that was Created
            for (int i=0; i<myArray.Length; i++)
            {
                Console.WriteLine("Array element {0}", myArray[i]);
            }

            // Array Analysis result
            Console.WriteLine("Array MAX element {0}, Array MIN element {1}, Array AVARAGE {2}, Sum of elements {3}", maxValue, minValue, avarageValue, SumValue);
            Console.ReadKey();
        }
    }
}
