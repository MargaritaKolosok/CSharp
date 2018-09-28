using System;


namespace Task_1

/*
 * Создать массив размера N элементов, заполнить его произвольными целыми значениями
 * (размер массива задает пользователь).  
    Вывести на экран: наибольшее значение массива,
    наименьшее значение массива,
    общую сумму всех элементов,
    среднее арифметическое всех элементов,
    вывести все нечетные значения. 

 * */
{
    class Program
    {
        
        //, out int min, out int sum, out int average, out int[] oddNum
        static void CreateArray(int arrayLength, out int max)
        {
            Random rand = new Random();
            int[] array = new int [arrayLength];
            
            for (int i=0; i<array.Length; i++)
            {
                array[i] = rand.Next();
                Console.WriteLine(array[i]);
               
            }
            
        }

        static void Main(string[] args)
        {

            //int[] array;
            int arrLength;
            int max;

            Console.WriteLine("Enter array lenght:");
            arrLength = Convert.ToInt32(Console.ReadLine());

            CreateArray(arrLength);

            Console.WriteLine();
            Console.ReadKey();


        }
    }
}
