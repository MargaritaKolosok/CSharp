using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Создайте расширяющий метод: public static T[] GetArray<T>(this MyList<T> list) 
Примените расширяющий метод к экземпляру типа MyList<T>,
разработанному в домашнем задании 2 для данного урока.
Выведите на экран значения элементов массива, который вернул расширяющий метод GetArray(). 
 */
namespace Task4
{
    class Program
    {
        static void Main(string[] args)
        {
            MyList<int> MyList = new MyList<int>();
            Random random = new Random();
            for (int i=0; i<10; i++)
            {                
                MyList.Add(random.Next(1,100));
            }

            Console.WriteLine(MyList.Count);
            int[] myArray = new int[MyList.Count];
            myArray = Extention.GetArray(MyList);

            for (int i = 0; i < 10; i++)
            {
                Console.Write(myArray[i] + " ");
            } 

            Console.ReadKey();
        }
    }
}
