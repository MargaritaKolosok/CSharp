using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsFiddle5
{
    class MyEventArgs : EventArgs
    {
        public int EventNum;
    }

    delegate void MyEventHandler(object source, MyEventArgs e);

    class MyEvent
    {
        static int count = 0;

        public event MyEventHandler SomeEvent;

        public void OnSomeEvent()
        {
            MyEventArgs arg = new MyEventArgs();

            if (SomeEvent!=null)
            {
                arg.EventNum = count++;
                SomeEvent(this, arg);
            }
        }
    }

    class X
    {
        public void Handler(object source, MyEventArgs e)
        {
            Console.WriteLine("Event from X class - Source {0}, Args {1}", source, e.EventNum);
        }        
    }

    class Program
    {
        public static void Handler1(object source, MyEventArgs e)
        {
            Console.WriteLine("Event from static Method - Source {0}, Args {1}", source, e.EventNum);
        }
        static void Main(string[] args)
        {
            MyEvent testEvent = new MyEvent();

            testEvent.SomeEvent += Handler1;

            X x = new X();

            testEvent.SomeEvent += x.Handler;

            testEvent.OnSomeEvent();
            testEvent.OnSomeEvent();
            testEvent.OnSomeEvent();
            testEvent.OnSomeEvent();

            Console.ReadKey();
        }
    }
}
