using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Напишите программу, в которой есть статический метод. Первым
аргументом статическому методу передается целочисленный массив. Вторым
аргументом статическому методу передается ссылка на другой метод. У 

метода-аргумента должен быть целочисленный аргумент, и он должен
возвращать целочисленный результат. Результатом статический метод
возвращает целочисленный массив. Элементы этого массива вычисляются
как результат вызова метода-аргумента, если ему передавать значения
элементов из массива-аргумента. Предложите механизм проверки
функциональности данного статического метода 
 * */
namespace Events_2
{
    public delegate int MyDeelegate(int x);

    
    class myClass
    {
        public static int[] Method(int[] array, MyDeelegate ob)
        {
            int[] Arr = new int[array.Length];

            for (int i = 0; i < array.Length; i++)
            {
                Arr[i] = ob(array[i]);
            }

            return Arr;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
