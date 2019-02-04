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
    
    }
public interface IMyList<T>
{
    void Add(T item);
    T this[int index] { get; }
    int Count { get; }
    void Clear();
    bool Contains(T item);
}

public class MyList<T> : IMyList<T>
{
    T[] array;

    public MyList()
    {
        array = new T[0];
    }

    public void Add(T element)
    {
        T[] temp = new T[array.Length];
        array.CopyTo(temp, 0);
        array = new T[array.Length + 1];
        temp.CopyTo(array, 0);
    }
    public int Count
    {
        get
        {
            return array.Length;
        }
    }

    public bool Contains(T item)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if ((object)array[i] == (object)item)
            {
                return true;
            }
        }
        return false;
    }

    public void Clear()
    {
        array = new T[0];
    }
    public T this[int index]
    {
        get
        {
            return array[index];
        }
    }

}
class CarCollection<T> : MyList<T>
    {
        List<string> CarNames;
        List<int> CarYear;
        public CarCollection()
        {
            CarNames = new List<string>();
            CarYear = new List<int>();
        }
       
        public void AddCar(string Name, int Year)
        {
            CarNames.Add(Name);
            CarYear.Add(Year);
        }
    public new string this[int index]
    {
        get
        {
            if (index < CarNames.Count)
            {                
                return Convert.ToString(CarNames[index] + " " + CarYear[index]);
            }
            return "No car with such index exists";
        }
    }
    }
    static class Extention
    {
        public static T[] GetArray<T>(this MyList<T> list)
            {
                T[] tempArray = new T[list.Count];
                for (int i=0; i<tempArray.Length;i++)
                {
                    tempArray[i] = list[i];
                }
        return tempArray;
            }
    }
    class Program
    {
        static void Main(string[] args)
        {
            CarCollection<string> Park = new CarCollection<string>();

            Park.AddCar("Toyota", 1992);
            Park.AddCar("Тойота", 2000);
            Park.AddCar("Форд", 1999);
            Park.AddCar("Мерседес", 2003);

            Console.WriteLine(Park[1]);
            Console.ReadKey();


        }
    }

