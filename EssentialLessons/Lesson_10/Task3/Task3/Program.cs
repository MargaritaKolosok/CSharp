using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Используя Visual Studio, создайте проект по шаблону Console Application.  
Создайте класс MyDictionary<TKey,TValue>.
Реализуйте в простейшем приближении возможность
использования его экземпляра аналогично экземпляру класса Dictionary (Урок 6 пример 5).
Минимально требуемый интерфейс взаимодействия с экземпляром,
должен включать метод добавления пар элементов,
индексатор для получения значения элемента по указанному индексу
и свойство только для чтения для получения общего количества пар элементов.  
 
 * */
namespace Task3
{
    class Program
    {
        static void Main(string[] args)
        {
            MyDictionary<string, string> myDictionary = new MyDictionary<string, string>();
            myDictionary.Add("table", "стол");
            myDictionary.Add("cow", "корова");
            myDictionary.Add("dog", "собака");
            myDictionary.Add("cat", "кот");
            myDictionary.Add("hourse", "лошадь");

            Console.WriteLine($"Number of elements in Dictionary is {myDictionary.Count}");

            Console.WriteLine(myDictionary["cow"]);
            Console.WriteLine(myDictionary["cowsss"]);

            myDictionary.Remove("cow");
            Console.WriteLine(myDictionary["cow"]);
            myDictionary.Remove("cow");
            Console.WriteLine(myDictionary["cow"]);

            Console.ReadKey();
           

        }
    }
}
