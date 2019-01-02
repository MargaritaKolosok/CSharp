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
       // revertArray.Show();
        
       
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
            
            Console.ReadKey();
        }
    }
}
