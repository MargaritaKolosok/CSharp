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
    public static TimeSpan DaysToBirthday(this DateTime birthday)
    {
        DateTime now = DateTime.Now;
        if (now.Date > birthday.Date)
        {
            return  now.Date - (birthday).Date;
        }
         return now.Date - (birthday).Date;
    }
}

namespace Dates1
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime day = new DateTime(1992,06,18);
            
            Person me = new Person("Rita", day, "+398298938938");
            Console.WriteLine(me.Birthday);
            Console.WriteLine(me.Name);
            Console.WriteLine(me.Phone);

            Console.WriteLine(day.DaysToBirthday());
            

            Console.ReadKey();
        }
    }
}
