using System;

delegate void myEventHandler();

class MyEvent
{
    public event myEventHandler SomeEvent;

    public void OnSomeEvent()
    {
        if (SomeEvent!=null)
        {
            SomeEvent();
        }
    }    
}
class EventDemo
{
    public static void Handler()
    {
        Console.WriteLine("This Event fired!");
    }
}

namespace Events
{
    class Program
    {
        static void Main(string[] args)
        {
            MyEvent newEvent = new MyEvent();
            newEvent.SomeEvent += EventDemo.Handler;
            newEvent.OnSomeEvent();
            Console.ReadKey();
        }
    }
}
