using System;

namespace Task_3
{
    /*
     * Создать метод, который будет выполнять увеличение длины массива
     * переданного в качестве аргумента, на 1 элемент.
     * Элементы массива, должны сохранить свое значение и порядок индексов. 
    Создайте метод, который принимает два аргумента, первый аргумент
    -  типа int [] array, второй аргумент - типа int value.
    В теле метода реализуйте возможность добавления второго аргумента метода
    в массив по индексу – 0, при этом длина нового массива, должна увеличиться на 1 элемент,
    а элементы получаемого массива в качестве первого аргумента должны скопироваться в новый массив начиная с индекса - 1. 

     * */
    class Program
    {
        static void PrintValues(int[] array)
        {
            foreach (int i in array)
            {
                Console.Write("\t{0}", i);
            }
            Console.WriteLine();
        }

        static void ResizeArray(ref int[] array, int elem)
        {
            int[] tempArray = new int [array.Length];
            tempArray = array;
            Array.Resize(ref array, array.Length +1);
            Array.Copy(tempArray, 0, array, 1, tempArray.Length);
            //tempArray.CopyTo(array,1);
            array.SetValue(elem, 0);
        }

        static void Main(string[] args)
        {
            int[] myArray = new int [5] {1,2,3,4,5};
            string toBeContinued = "yes";
            while (toBeContinued=="yes")
            {
                PrintValues(myArray);

                Console.WriteLine("Element to add in Array:");
                int element = Convert.ToInt32(Console.ReadLine());

                ResizeArray(ref myArray, element);
                Console.WriteLine("New Array:");
                PrintValues(myArray);

                Console.WriteLine("To continue program write 'yes'");
                toBeContinued = Convert.ToString(Console.ReadLine());
            }
            
            //PrintValues(myArray);

            //Console.ReadKey();


        }
    }
}
