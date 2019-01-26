using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Используя Visual Studio, создайте проект по шаблону Console Application.  
Создайте класс MyList<T>.
Реализуйте в простейшем приближении возможность использования его
экземпляра аналогично экземпляру класса List<T>.
Минимально требуемый интерфейс взаимодействия с экземпляром,
должен включать метод добавления элемента, индексатор для получения значения
элемента по указанному индексу и свойство только для чтения для получения общего количества элементов.  
 */


namespace Task2
{    
    class Program
    {
        static void Main(string[] args)
        {
            MyList<string> strArray = new MyList<string>();

            strArray.Add("1");
            strArray.Add("2");
            strArray.Add("3");
            Console.WriteLine(strArray[0]);
            Console.WriteLine(strArray[1]);
            Console.WriteLine(strArray[2]);
            Console.WriteLine($"Array length is {strArray.Count}");
            Console.WriteLine($"Array contains 1? {strArray.Contains("1")}");
            Console.WriteLine($"Array contains 4? {strArray.Contains("4")}");

            Random t = new Random();
            for (int i=0; i<10; i++)
            {
                strArray.Add(Convert.ToString(t.Next(100)));
            }

            Console.WriteLine(strArray.Count);
            Console.WriteLine(strArray.ToString());
            Console.ReadKey();
        }
    }
}
