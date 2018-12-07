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
class myClass
  {
    int x, y;
    static public myClass Factory(int x, int y)
        {
            myClass t = new myClass();
            t.x = x;
            t.y = y;
            return t;
        }
    public void Show()
    {
        Console.WriteLine("x {0}, y {1}", x, y);
    }
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

            for (int i=0; i<10; i++)
            {
                myClass my = myClass.Factory(i, i*2);
                my.Show();                
            }
            Console.ReadKey();
        }
    }
}
