using System;

namespace NumberConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] digits = { "нoль", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };

            int num = Convert.ToInt32(Console.ReadLine());

            int stringLength = (num.ToString()).Length;
            string newString = "";
            string stringNum = num.ToString();
          
             for(int i=0; i<stringLength; i++)
              {
                int c = int.Parse(Convert.ToString(stringNum[i]));
                newString += digits[c] + " ";
              }

            Console.WriteLine(newString);
            Console.ReadKey();
        }
    }
}
