using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Дата (три числа):
день, месяц, год
Список друзей: ФИО,
телефон, дата рождения,
Количество дней до дня
очередного рождения */

class Person
{
    string phone;
    DateTime birthday;
    public string Name;

    public DateTime Birthday
    {
        get { return birthday; }
    }

    public string Phone
        {
         get {return phone;}
        }

    public Person(string Name, DateTime birthday, string phone)
    {
        this.Name = Name;
        this.birthday = birthday;
        this.phone = phone;
    }

}

public static class ExtensionMethods
{
    public static int DaysToBirthday(this DateTime birthday)
    {
        DateTime now = DateTime.Now;
        DateTime birth = new DateTime(now.Year, birthday.Month, birthday.Day);

        if (now > birth)
        {            
            DateTime birth2 = new DateTime(now.Year+1, birthday.Month, birthday.Day);
            return  (int)(birth2 - now).TotalDays;
        }
         return (int)(birth - now).TotalDays;
    }
}

namespace Dates1
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime day = new DateTime(1992,12,28);
            
            Person me = new Person("Rita", day, "+398298938938");
            Console.WriteLine(me.Birthday);
            Console.WriteLine(me.Name);
            Console.WriteLine(me.Phone);

            Console.WriteLine(day.DaysToBirthday());            

            Console.ReadKey();
        }
    }
}
