using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public static void GetTrainByNum(this Train[] trains, int num)
    {
        for (int i=0; i< trains.Length; i++)
        {
            if (trains[i].Number == num)
            {
                trains[i].Show();
                return;
            }        
        }

        Console.WriteLine("No such Train exist!");
    }

    public static void Show(this Train train)
    {
        Console.WriteLine("City: {0}, number: {1}, Date: {2}", train.City, train.Number, train.Time);
    }

    public static void AddTrains(this Train[] trains)
    {

        for (int i=0; i<trains.Length; i++)
        {
            Console.WriteLine("Enter city destination:");
            string city = Convert.ToString(Console.ReadLine());

            Console.WriteLine("Enter train number:");
            int number = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Enter date:");
            var d = Console.ReadLine();

            DateTime time = (DateTime.TryParse(d, out DateTime result))? result : DateTime.Now;
            trains[i] = new Train(city, number, time);
        }
    }
}

namespace Task_2_1
{
    class Program
    {
        static void Main(string[] args)
        {
            Train[] trains = new Train[2];
            Schedule.AddTrains(trains);
            Schedule.GetTrainByNum(trains, 33);
            Console.ReadKey();
        }
    }
}
