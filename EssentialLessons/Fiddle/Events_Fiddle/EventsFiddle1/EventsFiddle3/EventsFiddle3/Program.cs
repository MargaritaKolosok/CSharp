using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsFiddle3
{
    public delegate void MyDelegate();

    class BaseClass
    {
        protected virtual event MyDelegate MyEvent = null;

        public virtual event MyDelegate VisibleEvent
        {
            add
            {
                MyEvent += value;
            }
            remove
            {
                MyEvent -= value;
            }
        }

        public void RunEvent()
        {
            MyEvent();
        }
    }
    class DerivedClass : BaseClass
    {               
        public override event MyDelegate VisibleEvent
        {
            add
            {
                MyEvent += delegate() { Console.WriteLine("From derived class"); };
            }
            remove
            {
                MyEvent -= null;
            }
        }
    }

    class Program
    {
        static void Method()
        {
            Console.WriteLine("Method");
        }

        static void Main(string[] args)
        {
            MyDelegate my = new MyDelegate(Method);

            BaseClass baseClass = new BaseClass();
            baseClass.VisibleEvent += Method;
            baseClass.RunEvent();

            Console.ReadKey();


        }
    }
}
