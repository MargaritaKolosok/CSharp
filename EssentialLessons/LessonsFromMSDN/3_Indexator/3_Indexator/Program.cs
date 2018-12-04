using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class DayOfTheWeek
{
    private string[] days = { "Sun", "Mon", "Tues", "Wed", "Thurs", "Fri", "Sat" };

    private int GetDay(string testDay)
    {

        for (int j = 0; j < days.Length; j++)
        {
            if (days[j] == testDay)
            {
                return j;
            }
        }

        throw new System.ArgumentOutOfRangeException(testDay, "testDay must be in the form \"Sun\", \"Mon\", etc");
    }

    public int this[string day]
    {
        get { return (GetDay(day)); }
    }
}

namespace _3_Indexator
{
    class Program
    {
        static void Main(string[] args)
        {
            DayOfTheWeek day = new DayOfTheWeek();
            Console.WriteLine(day["Sun"]);
            Console.ReadKey();
        }
    }
}
