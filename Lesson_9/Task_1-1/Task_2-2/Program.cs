using System;

namespace Task_2_2
{

    /*
     *  
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
        static int[] CreateArray(int[] array, int arrLength)
        {
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = arrLength * (i + 1) + 2 * i;
            }

            return array;
        }

       static int[] SubArray(int[] array, int index, int count, out int[] mySubArray)
        {
            int j = 0;
            int[] subArray = new int[count];

            for (int i=0; i<array.Length; i++)
            {
                if (i >= index && i < count)
                {
                    subArray[j] = array[i];
                    j++;
                }
                else if (i<count && i>=array.Length)
                {
                    subArray[i] = 1;
                    j++;

                }
                
            }
            mySubArray = subArray;
            return mySubArray;

        }
    

        static void Main(string[] args)
        {
            Console.WriteLine("Enter number of array elements:");
            int arrLength = Convert.ToInt32(Console.ReadLine());

            int[] myArray = new int[arrLength];

            myArray = CreateArray(myArray, arrLength);

            for (int i = 0; i < myArray.Length; i++)
            {
                Console.Write("{0}, ", myArray[i]);
            }
            Console.WriteLine("Enter Index:");
            int myIndex = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter Count:");
            int myCount= Convert.ToInt32(Console.ReadLine());
            SubArray(myArray, myIndex, myCount, out int[] mySubArray);

            for (int i = 0; i <mySubArray.Length; i++)
            {
                Console.Write("{0}, ", mySubArray[i]);
            }
            
            Console.ReadKey();
        }
    }
}
