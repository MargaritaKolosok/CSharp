using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsFiddle1
{
    public delegate void EventDelegate();

    public class MyClass
    {
        public event EventDelegate MyEvent = null;

        public void InvokeEvent()
        {
            MyEvent.Invoke();
        }
    }
    class Program
    {
        static private void Handler1()
        {
            Console.WriteLine("Handler1");
        }
        static private void Handler2()
        {
            Console.WriteLine("Handler2");
        }

        static void Main(string[] args)
        {
            MyClass instance = new MyClass();

            instance.MyEvent += new EventDelegate(Handler1); // подписка на событие
            instance.MyEvent += Handler2;

            instance.MyEvent += delegate () { Console.WriteLine("Inline delegate"); };

            instance.InvokeEvent();
            Console.ReadKey();
        }
    }
}
