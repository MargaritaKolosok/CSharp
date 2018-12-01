using System;

namespace NumberConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] digits = { "нoль", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять" };
            string newString = "";

            Console.WriteLine("Введите число для конвертации в буквенное представление:");

            int num = Convert.ToInt32(Console.ReadLine());
            string stringNum = num.ToString();
          
             for(int i=0; i<stringNum.Length; i++)
              {
                int c = int.Parse(Convert.ToString(stringNum[i]));
                newString += digits[c] + " ";
              }

            Console.WriteLine(newString);
            Console.ReadKey();
        }
    }
}
