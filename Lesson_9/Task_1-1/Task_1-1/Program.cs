using System;

namespace Task_1_1
{
    
    class Program
    {   
        // Create First Array with set by User arrLength, and random values
        static int[] CreateArray(int[] array, int arrLength)
        {
            for (int i=0; i<array.Length; i++)
            {
                array[i] = arrLength * (i+1) + 2*i;
            }

            return array;
        }

        // Check max, min, avarage, Sum of elements of the array
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
        // Check number of even numbers in the array
        static int EvenNum(int[] array, out int evenNum)
        {
            evenNum = 0;

            for (int i=0; i<array.Length; i++)
            {
                if (IsEven(array[i]))
                {
                    evenNum++;
                }
            }
            return evenNum;
        }
        // Check is array elements value Even
        static bool IsEven(int Num)
        {
            bool result = false;
           
                if (Num % 2 != 0)
                {
                   result = true;
                }
                else
                {
                    result = false;
                }

            return result;
        }

        // Create new Even array with length equal to its even elements. Save even elements of the array to even array
        static void CreateEvenArray(int[] array, ref int evenNum, out int[] myEvenArray)
        {
            int[] evenArray = new int[evenNum];
            int j = 0;
            for (int i=0; i<array.Length; i++)
            {
                if (IsEven(array[i]))
                {
                    evenArray[j] = array[i];
                    j++;
                }
                
            }
            myEvenArray = evenArray;
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

            Console.WriteLine("Even numbers in array {0}", EvenNum(myArray, out int evenNumbers));
            CreateEvenArray(myArray, ref evenNumbers, out int[] myEvenArray);

            for (int i = 0; i < myEvenArray.Length; i++)
            {
                Console.WriteLine("Array EVEN element {0}", myEvenArray[i]);
            }

            Console.ReadKey();
        }
    }
}
