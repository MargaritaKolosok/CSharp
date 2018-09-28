using System;

namespace Task_1_1
{
    
    class Program
    {
        static int[] ModifyArray(int[] array, int modifier)
        {
            for (int i=0; i<array.Length; i++)
            {
                array[i] = modifier * i;
            }

            return array;
        }

        static void Main(string[] args)
        {
            
                       
            Console.WriteLine("Enter number of array elements:");
            int modifier = Convert.ToInt32(Console.ReadLine());
            int[] myArray = new int [modifier];
            myArray = ModifyArray(myArray, modifier);
            for (int i=0; i<myArray.Length; i++)
            {
                Console.WriteLine("Array element {0}", myArray[i]);
            }
            
            Console.ReadKey();
        }
    }
}
