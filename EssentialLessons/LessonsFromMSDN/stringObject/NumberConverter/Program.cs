using System;

namespace NumberConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] digits = { "нуль", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };

            int num = Convert.ToInt32(Console.ReadLine());

            int stringLength = (num.ToString()).Length;

            string newString = "";
            string stringNum = num.ToString();
          //  int[] myNum = int.Parse(stringNum);

            for(int i=0; i<stringLength; i++)
            {
              newString += stringNum.ChartAt(i);
            }

            Console.WriteLine(newString);
            Console.ReadKey();
        }
    }
}
