using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 Создать класс Employee.
В теле класса создать пользовательский конструктор, который принимает два строковых аргумента, и
инициализирует поля, соответствующие фамилии и имени сотрудника.
Создать метод рассчитывающий оклад сотрудника (в зависимости от должности и стажа) и налоговый
сбор.
Написать программу, которая выводит на экран информацию о сотруднике (фамилия, имя, должность),
оклад и налоговый сбор. 
 * */
class Employee
{
    private string name, surname;
  //  public double Tax, Sallary;


    public Employee(string name, string surname)
    {
        this.name = name;
        this.surname = surname;
    }
    public double CountSallary(string position, int experience)
    {
        double sallary;
        double baseSallary = 32000;
        if (position == "QA")
        {
            sallary = (experience + 0.1) * 0.2 * baseSallary;

        }
        else if (position == "MANAGER")
        {
            sallary = (experience + 0.1) * 0.37 * baseSallary;
        }
        else if (position == "PROGRAMMER")
        {
            sallary = (experience + 0.1) * 0.46 * baseSallary;
        }
        else
        {
            sallary = (experience + 0.1) * 0.11 * baseSallary;
        }
        // 
        return sallary;

    }
    public void CountTax(double sallary)
    {
        Console.WriteLine((sallary / 100) * 5.5); 
    }


    public void getInfo()
    {
        Console.WriteLine("Employee {0} {1} has sallary", name, surname);
    }
            
}
class Sallary
{

}

namespace EmployeeSallary
{
    class Program
    {
        static void Main(string[] args)
        {
            Employee John = new Employee("John", "Smith");
            double emSallary = John.CountSallary("QA", 4);
           

            

            John.getInfo();
            John.CountTax(emSallary);


            Console.ReadKey();

        
            

        }
    }
}
