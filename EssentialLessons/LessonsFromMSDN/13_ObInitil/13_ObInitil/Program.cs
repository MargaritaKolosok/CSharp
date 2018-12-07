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
static class Numeric
{
    static public int Num(double num)
    {
        return (int)num;
    }
    static public double Part(double num)
    {
        return num - (int)num;
    }
    static public bool IsEven(double num)
    {
        return (num%2)==0 ? true : false;
    }
    static public bool IsOdd(double num)
    {
        return !IsEven(num);
    }

}

namespace _13_ObInitil
{
    class Program
    {
        // Person

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

        // Static Method in myClass

            for (int i=0; i<10; i++)
            {
                myClass my = myClass.Factory(i, i*2);
                my.Show();                
            }

        // Static Class

            Console.WriteLine(Numeric.Num(34.88));
            Console.WriteLine(Numeric.Part(34.88));
            Console.WriteLine(Numeric.IsEven(34.88));
            Console.WriteLine(Numeric.IsOdd(34.88));


            Console.ReadKey();
        }
    }
}
