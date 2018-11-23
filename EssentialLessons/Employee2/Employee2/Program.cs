using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Sallary
{
    public int baseSallary = 32000;
    double sallary, tax, experience;

    public Sallary(double experience)
    {
        this.experience = experience;
    }

    public double SallarySize
    {
    get {
            sallary = baseSallary * 0.2 * experience;
            return sallary;
        }
               
    }
    public double TaxSize
    {
       get
        {
            tax = (sallary / 100) * 25;
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

       // Console.Write("Position:");
       // string position = Convert.ToString(Console.ReadLine());

        Sallary sallary = new Sallary(experience);
       

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
