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
        static void CreateArray(int arrayLength, out int[] array)
        {
            //Random rand = new Random();
            int[] userArray = new int [arrayLength];


                for (int i=0; i<userArray.Length; i++)
                {
                  userArray[i] = i*2;
                //  Console.WriteLine(userArray[i]);
                
                }

                array = userArray;


        }

        static void Main(string[] args)
        {

            int[] arrayWrite;
            int arrLength;
           // int max;

            Console.WriteLine("Enter array lenght:");
            arrLength = Convert.ToInt32(Console.ReadLine());

            CreateArray(arrLength, out int[] array);

            Console.WriteLine();
            Console.ReadKey();


        }
    }
}
