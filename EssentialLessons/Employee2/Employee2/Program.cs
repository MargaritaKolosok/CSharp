using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Sallary
{
    private int baseSallary = 32000;
    double sallary, tax, experience;
    private string position;
    private double coeficient = 0.1;

    public Sallary(double experience, string position)
    {
        this.experience = experience;
        this.position = position;
    }

    public double SallarySize
    {
        get {
            switch (position)
            {
                case "QA":
                    coeficient = 0.3;
                    break;
                case "MANAGER":
                    coeficient = 0.45;
                    break;
                case "PROGRAMMER":
                    coeficient = 0.66;
                    break;
                default:
                    coeficient = 0.15;
                    break;
            }

            sallary = baseSallary * experience * coeficient;
            return sallary;

            }               
    }

    public double TaxSize
    {
       get
        {
            tax = (sallary / 100) * 5.5;
            return tax;
        }
    }
  }

class Employee
{
    private string name, surname;

    public Employee(string name, string surname)
        {
            this.name = name;
            this.surname = surname;
        }

    public void CountTaxes()
    {
        Console.WriteLine("Hello {0}, please enter year of experience and Position enter ", name);
        Console.Write("Experience years:");
        double experience = Convert.ToInt32(Console.ReadLine());

        Console.Write("Position:");
        string position = Convert.ToString(Console.ReadLine());

        Sallary sallary = new Sallary(experience, position);
        Console.WriteLine("Sallary: {0}, Tax: {1}", sallary.SallarySize, sallary.TaxSize);
    }       
}

namespace Employee2
{
    class Program
    {
        static void Main(string[] args)
        {
            Employee John = new Employee("John", "Smith");
            John.CountTaxes();
            Console.ReadKey();
        }
    }
}
