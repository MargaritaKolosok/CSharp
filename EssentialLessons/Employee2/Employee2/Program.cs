using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Sallary
{
    int baseSallary = 32000;
    double sallary=1, tax=1, experience;

    public Sallary(double experience)
    {
        this.experience = experience;
    }

    public double SallarySize
    {
        set { sallary = baseSallary * 0.2 * experience; }
        get { return sallary; }
               
    }
    public double TaxSize
    {
        set { tax = (sallary /100)*25; }
        get { return tax; }
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
       // Console.Write("Position:");
       // string position = Convert.ToString(Console.ReadLine());

        Sallary sallary = new Sallary(experience);
        Console.WriteLine("Sallary: {0}, Tax: {1}", sallary.SallarySize, sallary.SallarySize);
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
