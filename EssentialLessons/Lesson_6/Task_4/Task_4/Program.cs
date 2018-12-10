using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Требуется: создать расширяющий метод для целочисленного массива, который сортирует элементы
массива по возрастанию.  */
static class Array
{
    public static int[] Sort(this int[] arr)
    {
        int[] newArr = new int[arr.Length];

        for (int i = 0; i < arr.Length; i++)
        {
            int a = i;

            for (int j = 0; j < arr.Length; j++)
            {

                int b = j;

                if (arr[a] < arr[b])
                {
                    int temp = arr[a];
                    arr[a] = arr[b];
                    arr[b] = temp;
                }
            }
        }
        return arr;
    }
}

namespace Task_4
{
    class Program
    {
       

        static void Main(string[] args)
        {
            int[] my = new int[] { 5,6,7,8,3,2,4,5,6,7};
            my.Sort();

            for(int i=0; i<my.Length;i++)
            {
                Console.WriteLine(" " + my[i]);
                
            }

            Console.ReadKey();
        }
    }
}
