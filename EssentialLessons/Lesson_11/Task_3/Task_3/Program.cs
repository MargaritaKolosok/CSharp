using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Используя Visual Studio, создайте проект по шаблону Console Application.
Создайте класс Dictionary<TKey,TValue>. Реализуйте в простейшем приближении возможность
использования его экземпляра аналогично экземпляру класса Dictionary из пространства имен
System.Collections.Generic. Минимально требуемый интерфейс взаимодействия с экземпляром,
должен включать метод добавления пар элементов, индексатор для получения значения элемента по
указанному индексу и свойство только для чтения для получения общего количества пар элементов.  
 * */

class Dictionary<TKey, TValue>
{
    TKey[] KeyArray = new TKey[0];
    TValue[] ValueArray = new TValue[0];

    public void Add(TKey key, TValue value)
    {
        TKey[] tempKey = new TKey[KeyArray.Length];
        TValue[] tempValue = new TValue[ValueArray.Length];

        KeyArray.CopyTo(tempKey, 0);
        ValueArray.CopyTo(tempValue, 0);

        KeyArray = new TKey[tempKey.Length +1];
        ValueArray = new TValue[tempValue.Length + 1];
        tempKey.CopyTo(KeyArray,0);
        tempValue.CopyTo(ValueArray,0);
        KeyArray[KeyArray.Length - 1] = key;
        ValueArray[ValueArray.Length - 1] = value;
    }

    public int Count
    {
        get { return KeyArray.Length; }        
    }

    public TKey this[int index]
    {
        get { return KeyArray[index]; }
    }
}
namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            dictionary.Add("one",1);
            dictionary.Add("two", 2);
            dictionary.Add("three", 3);
            dictionary.Add("four", 4);

            Console.WriteLine(dictionary.Count);
            Console.WriteLine(dictionary[1]);
            Console.ReadKey();
        }
    }
}
