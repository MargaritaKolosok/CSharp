using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Создайте расширяющий метод: public static T[] GetArray<T>(this MyList<T> list) 
Примените расширяющий метод к экземпляру типа MyList<T>,
разработанному в домашнем задании 2 для данного урока.
Выведите на экран значения элементов массива, который вернул расширяющий метод GetArray(). 
 */
namespace Task_4
{
    static class ExtentionMethod
    {
        public static T[] GetArray<T>(this List<T> list)
        {
            T[] tempArray = new T[list.Count];
            int index = 0;
            foreach (T t in list)
            {
                tempArray[index] = list[index];
                index++;
            }
            return tempArray;
        }
    }
    
    class Program
    {
        
        static void Main(string[] args)
        {
            List<string> stringList = new List<string>();
            stringList.Add("1");
            stringList.Add("2");
            stringList.Add("3");
            stringList.Add("4");
            string[] array = new string[4];
            array = ExtentionMethod.GetArray(stringList);
        }
    }
}
