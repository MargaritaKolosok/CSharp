using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
Требуется: Описать структуру с именем Train, содержащую следующие поля: название пункта
назначения, номер поезда, время отправления.
Написать программу, выполняющую следующие действия:
- ввод с клавиатуры данных в массив, состоящий из восьми элементов типа Train (записи должны быть
упорядочены по номерам поездов);
- вывод на экран информации о поезде, номер которого введен с клавиатуры (если таких поездов нет,
вывести соответствующее сообщение). 
 * 
 */
struct Train
{
    string city;
    int number;
    DateTime time;

    public Train(string city, int number, DateTime time)
    {
        this.city = city;
        this.number = number;
        this.time = time;
    }

    public string City
    {
        get { return city; }
    }
    public int Number
    {
        get { return number; }

    }
    public DateTime Time
    {
        get { return time; }
    }
}

static class Schedule
{
   public static void AddTrains(Train[] trains)
    {       
        for (int i=0; i<trains.Length; i++)
        {             
            Console.WriteLine("City:");
            string City = Console.ReadLine();

            Console.WriteLine("Number:");
            int Number = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Time:");
            var d = Console.ReadLine();
            DateTime Time = DateTime.Parse(d);

            trains[i] = new Train(City, Number, Time);
        }        
    }
    public static void GetTrain(this Train[] trains, int num)
    {
        for (int i = 0; i < trains.Length; i++)
        {
            if (trains[i].Number == num)
            {
                Console.WriteLine("Train with number {0} was found", trains[i].Number);
            }
            else
            {
                Console.WriteLine("No such train number");
            }
        }       
    }
}



namespace Task_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Train[] trains = new Train[1];

            Schedule.AddTrains(trains);
            Schedule.GetTrain(trains, 33);
            
            Console.ReadKey();
        }
    }
}
