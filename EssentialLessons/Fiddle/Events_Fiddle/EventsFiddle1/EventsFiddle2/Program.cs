using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsFiddle2
{
    public delegate void MyDelegate();

    public class myClass
    {
        MyDelegate myEvent = null;

        public event MyDelegate MyEvent
        {
            add
            {
                myEvent += value;
            }
            remove
            {
                myEvent -= value;
            }
        }

        public void InvokeEvent()
        {
            if (myEvent == null)
            {
                Console.WriteLine("Nothing attached to event");
            }
            else
            {
                myEvent.Invoke();
            }
            
        }
    }
    class Program
    {
        public static void Method1()
        {
            Console.WriteLine("Hey");
        }
        static void Main(string[] args)
        {
            myClass instance = new myClass();
            instance.MyEvent += Method1;

            instance.InvokeEvent();

            instance.MyEvent -= Method1;
            instance.InvokeEvent();

            Console.ReadKey();
        }
    }
}
