using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsFiddle7
{
    // Class that holds Customg Event info
    class CustomEventArgs : EventArgs
    {
        string message;

        public CustomEventArgs(string message)
        {
            this.message = message;
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    class Publisher
    {
        public event EventHandler<CustomEventArgs> RaiseCustomEvent;

        public void DoSomething()
        {
            CustomEventArgs args = new CustomEventArgs("Something was done");
            OnRaiseCustomEvent(args);
        }

        protected virtual void OnRaiseCustomEvent(CustomEventArgs e)
        {
            EventHandler<CustomEventArgs> handler = RaiseCustomEvent;

            e.Message += "Date and Time now" + DateTime.Now;

            handler?.Invoke(this, e);
        }
    }

    class Subsriber
    {
        string id;

        public Subsriber(string id, Publisher pub)
        {
            this.id = id;
            pub.RaiseCustomEvent += HandleCustomEvent;
            
        }
        void HandleCustomEvent(object sender, CustomEventArgs e)
        {
            Console.WriteLine($"Event id is {id}, date time message is following: {e.Message}");
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Publisher pub = new Publisher();
            Subsriber sub1 = new Subsriber("Hello worlds", pub);
            Subsriber sub2 = new Subsriber("One more strng", pub);

            pub.DoSomething();

            Console.ReadKey();
        }
    }
}
