using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Adress
{
    public int Index { set; get; }
    public string Country { set; get; }
}

namespace LessonFromCourcev2
{
    class Program
    {
        static void Main(string[] args)
        {
            Adress myHome = new Adress();
            myHome.Country = "Ukraine";
            myHome.Index = 12345;

            Console.WriteLine("My home adress is {0} {1}", myHome.Index, myHome.Country);
            Console.ReadKey();

        }
    }
}
