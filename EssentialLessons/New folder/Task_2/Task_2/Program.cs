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
    public string City;
    public int Number;
    public double Time;
    
}
class Schedule
{
    private Train[] trains;

    public void AddTrain(int num)
    {
        trains = new Train[num];
        for (int i=0; i<num; i++)
        { 
            Train t = new Train();
            Console.WriteLine("City:");
            t.City = Console.ReadLine();
            Console.WriteLine("Number:");
            t.Number = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Time:");
            t.Time = Convert.ToDouble(Console.ReadLine());
            trains[i] = t;
        }
        
    }
    
  
}
static class Trains
    {
    public static Train GetTrain(this List<Train> list)
    {
        Train t = new Train();
        return t;
    }
}


namespace Task_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Schedule my = new Schedule();
            my.AddTrain(3);
            Console.ReadKey();
        }
    }
}
