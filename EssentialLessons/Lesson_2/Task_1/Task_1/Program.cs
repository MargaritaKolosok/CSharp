using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task_1
{
    public class Building
    {
       public int Area;
       public int Person;
       public int AreaPerPerson;

        public int CountArea(int area, int personNumber)
        {
            AreaPerPerson = area / personNumber;
            return AreaPerPerson;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Building flat = new Building();

            flat.Area = 50;
            flat.Person = 3;

            Console.WriteLine("Area per person is equal to {0}", flat.CountArea(flat.Area, flat.Person));
            Console.ReadKey();


        }
    }
}
