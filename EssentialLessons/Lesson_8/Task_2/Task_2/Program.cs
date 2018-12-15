using System;
/*
Используя Visual Studio, создайте проект по шаблону Console Application.  
Реализуйте программу, которая будет принимать от пользователя дату его рождения и выводить количество дней до его следующего дня рождения. 
 
 * */
static class CountDays
{
    public static void DaysToBirthday(string str)
    {
        if (DateTime.TryParse(str, out DateTime Birthday))
        {
            DateTime today = DateTime.Now;
            TimeSpan DaysToBirthday = today - Birthday;
            Console.WriteLine("Days left to Birthday {0}", DaysToBirthday);
        }
        else
        {
            Console.WriteLine("Not valid Day entered!");
        }
       
    }
}

namespace Task_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Date of birth");
            string birth = Console.ReadLine();
            CountDays.DaysToBirthday(birth);
            Console.ReadKey();
        }
    }
}
