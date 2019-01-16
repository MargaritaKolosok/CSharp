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
        public event EventDelegate myEvent = null;
    }
    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }
}
