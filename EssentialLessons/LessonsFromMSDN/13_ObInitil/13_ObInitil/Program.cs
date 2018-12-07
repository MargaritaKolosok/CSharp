using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class Person
{
    public string Name;
    public int age;
}
namespace _13_ObInitil
{
    class Program
    {
        static bool IsAdult(int baseAge, int age)
        {
            if (age > baseAge)
                return true;
            return false;
        }
        static void Main(string[] args)
        {
            Person Rita = new Person { Name="Rita", age=26 };
            
            Console.WriteLine(Rita.age);
            Console.WriteLine(IsAdult(baseAge: 18, age: 26));
            Console.ReadKey();
        }
    }
}
