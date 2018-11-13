using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Building
    {
        public int Floors;
        public int Occupants;
        public int Area;

    public int AreaPerPerson()
        {
        return Area / Occupants;
        }
    public int MinArea(int minArea)
    {
        return Area / minArea;
    }

    }

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Building Office = new Building();
            Building House = new Building();

            int minArea = 5;

            Office.Area = 100;
            Office.Occupants = 12;
            Console.WriteLine("Max person number possible to be in office is {0}", Office.MinArea(minArea));
            Console.WriteLine("Min Area Per person is {0}", Office.AreaPerPerson());
            Console.ReadKey();
        }
    }
}
