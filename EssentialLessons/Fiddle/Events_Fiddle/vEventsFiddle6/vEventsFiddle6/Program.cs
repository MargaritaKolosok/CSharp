using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vEventsFiddle6
{
    class EventHandlerArgs : EventArgs
    {
        public int Day
        {
            get { return Day; }
            private set { Day = Date.DayOfYear; }
        }
        public DateTime Date
        {
            get { return Date; }
            set { Date = DateTime.Now; }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
