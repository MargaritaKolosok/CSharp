using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2_try
{
    interface ICar
    {
        string Name { get; }
        int Date { get; }
    }
    interface IMyList<T>
    {
        void Add(T item);
        T this[int index] { get; }
        int Count { get; }
    }

    class Car : ICar
    {
        private string name;
        private int date;

        public Car(string Name, DateTime Date)
        {
            name = Name;
            date = Date.Year;
        }
        public string Name
        {
            get
            {
                return name;
            }
        }
        public int Date
        {
            get
            {
                return date;
            }
        }
    }

    class MyList<T> : IMyList<T>
    {
        List<T> CarList;

        public MyList()
        {
            CarList = new List<T>();
        }
        public void Add(T item)
        {
            CarList.Add(item);
        }        

        public int Count
        {
            get { return CarList.Count; }
        }       

        public T this[int index]
        {
            get
            {
                return CarList[index];
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Car car1 = new Car("CAR Name", new DateTime(1992,2,2));
            Car car2 = new Car("CArAnme", new DateTime(1992,2,2));

            MyList<Car> myList = new MyList<Car>();
            myList.Add(car1);
            myList.Add(car2);

            Console.WriteLine(myList[0].Name + " " + myList[0].Date);
            Console.ReadKey();
        }
    }
}
