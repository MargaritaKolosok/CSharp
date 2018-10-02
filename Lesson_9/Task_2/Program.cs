using System;

namespace Task_2
{

    /*
     1) Создать метод MyReverse(int [] array),
     который принимает в качестве аргумента массив целочисленных элементов и возвращает инвертированный массив
     (элементы массива в обратном порядке).  
    2) Создайте метод int []  SubArray(int [] array, int index, int count).
    Метод возвращает часть полученного в качестве аргумента массива, начиная с позиции указанной в аргументе index,
    размерностью, которая соответствует значению аргумента count.  
 
    Если аргумент count содержит значение больше чем количество элементов,
    которые входят в выбираемую часть исходного массива (от указанного индекса index, до индекса последнего элемента),
    то  при формировании нового массива размерностью в count, заполните единицами те элементы,
    которые не были скопированы из исходного массива. 

         */
    class Program
    {
        // Create array with set Length, set random values
        static int[] CreateArray(int[] array, int arrLength)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = arrLength * (i + 1) + 2 * i;
            }

            return array;
        }

        // Reverse array and return reversed array
       static void MyReverse(int[] array, out int[] myReverseArray)
        {
            int[] reverseArray = new int[array.Length];
            int j = array.Length - 1;
            for (int i=0; i<array.Length; i++)
            {
                reverseArray[j] = array[i];
                j--;
            }
            myReverseArray = reverseArray;           
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of array elements:");            
            int arrLength = Convert.ToInt32(Console.ReadLine());
            
            int[] myArray = new int[arrLength];

            myArray = CreateArray(myArray, arrLength);

            for (int i = 0; i < myArray.Length; i++)
            {
                Console.WriteLine("Array element {0}", myArray[i]);
            }
            
            MyReverse(myArray, out int[] myReverseArray);
            
            for (int i = 0; i < myReverseArray.Length; i++)
            {
                Console.WriteLine("reverseArray element {0}", myReverseArray[i]);
            }
            Console.ReadKey();
        }
    }
}
