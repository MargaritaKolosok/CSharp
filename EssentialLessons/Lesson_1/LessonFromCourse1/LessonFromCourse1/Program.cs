using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*

    Используя Visual Studio, создайте проект по шаблону Console Application.
Требуется:
Создать класс с именем Address.
В теле класса требуется создать поля: index, country, city, street, house, apartment. Для каждого
поля, создать свойство с двумя методами доступа.
Создать экземпляр класса Address.
В поля экземпляра записать информацию о почтовом адресе.
Выведите на экран значения полей, описывающих адрес. 

    */

class Adress
{
  public int index;
  public string country;
  public string city;
  public string street;
  public string house;
  public int apartment;

    public Adress(int index, string country, string city, string street, string house, int apartment)
    {
        this.index = index;
        this.country = country;
        this.city = city;
        this.street = street;
        this.house = house;
        this.apartment = apartment;
    }

}

namespace LessonFromCourse1
{
    class Program
    {
        static void Main(string[] args)
        {
            Adress myHomeAdress = new Adress(12345, "Ukraine", "Kiev", "BlaBlakara", "34a", 355);
            Console.WriteLine(myHomeAdress.index + " " + myHomeAdress.country);
            Console.ReadKey();
        }
    }
}
