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

            Console.WriteLine("Enter Area");
            house.Area = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter Anumber of persons");
            house.Person = Convert.ToInt32(Console.ReadLine());

            house.CountArea();

            Console.ReadKey();
           
        }
    }
}
