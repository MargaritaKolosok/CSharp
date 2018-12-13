using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    struct Train
    {
        string city;
        int number;
        DateTime time;

        public Train(string city, int number, DateTime time)
        {
            this.city = city;
            this.number = number;
            this.time = time;
        }

        public string City
        {
            get { return city; }
        }
        public int Numver
        {
            get { return number; }

        }
        public DateTime Time
        {
            get { return time; }
        }        
    }
}
