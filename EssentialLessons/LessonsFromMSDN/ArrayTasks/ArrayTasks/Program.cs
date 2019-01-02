using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Введите массив символов из 5 элементов. Определите, является ли он
палиндромом (то есть при чтении наоборот содержание не изменяется,
например, слово `БОБ').
 * 
 * */
delegate void ShowArray(string[] array);
static class ExtendedMethods
{
    public static bool IsPolindrom(string[] array)
    {
        string[] revertArray = new string[array.Length];
        
        for (int i = 0; i < array.Length; i++)
        {
            revertArray[i] = array[i];
        }

        Array.Reverse(revertArray);
              
        if (revertArray == array)
        {
            return true;
        }
        else
        {
            return false;
        }       
    }

    public static void Show(this string[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Console.WriteLine(array[i]);
        }
    }

    public static string[] Revert(this string[] array)
    {
        Array.Reverse(array);
        return array;
    }

    
}
class Ex
{
    public static void ShowArr(string[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            Console.WriteLine(array[i]);
        }
    }
}


namespace ArrayTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] myArray = { "Hello", "my", "World" };
            Console.WriteLine(ExtendedMethods.IsPolindrom(myArray));

            myArray.Show();

            myArray.Revert();
            myArray.Show();

            Console.WriteLine();
            ShowArray ShowArrayNow = Ex.ShowArr;

            ShowArrayNow(myArray);
            
            Console.ReadKey();
        }
    }
}
