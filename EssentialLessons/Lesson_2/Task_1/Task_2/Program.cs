using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_2
{
    public class Building
    {
        public int Area;
        public int Person;

        public void CountArea()
        {       
           Console.WriteLine("Area per person {0}", Area / Person);
         
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Building house = new Building();
            house.Area = 500;
            house.Person = 20;
            house.CountArea();

            Console.ReadKey();
           
        }
    }
}
