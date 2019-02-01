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

    }
}
namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
