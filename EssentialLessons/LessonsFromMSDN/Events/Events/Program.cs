using System;

delegate void myEventHandler();
delegate void NumEventHandler(int x);

class MyEvent
{
    public event myEventHandler SomeEvent;
    
    public void OnSomeEvent()
    {
        SomeEvent?.Invoke();
    }

    public event NumEventHandler NumEvent;

    public void OnNumEvent(int x)
    {
        NumEvent?.Invoke(x);
    }
    
}
class EventDemo
{
    public static void Handler()
    {
        Console.WriteLine("This Event fired!");
    }
    public static void NumHandler(int x)
    {
        Console.WriteLine(x);
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

            newEvent.NumEvent += EventDemo.NumHandler;
            newEvent.OnNumEvent(3);
            Console.ReadKey();
        }
    }
}
