using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Создайте класс CarCollection<T>. Реализуйте в простейшем приближении возможность
использования его экземпляра для создания парка машин. Минимально требуемый интерфейс
взаимодействия с экземпляром, должен включать метод добавления машин с названием машины и
года ее выпуска, индексатор для получения значения элемента по указанному индексу и свойство
только для чтения для получения общего количества элементов.
Создайте метод удаления всех машин автопарка. 
 */
namespace Task_2
{
    interface Car
    {        
        void Add(string Name, int Year);
        Car this[int index] { get; }
        int Count { get; }        
    }

    class CarCollection<T> where T : Car
    {
        List<T> CarList = new List<T>();
        public void Add(string Name, int Year)
        {
           
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
