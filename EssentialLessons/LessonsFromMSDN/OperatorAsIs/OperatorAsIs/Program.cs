﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class Person
{
   public string Name;
}
class Child : Person
{
    int Age;
}
class Flower
{
    string Name;
}
namespace OperatorAsIs
{
    class Program
    {
        static void Main(string[] args)
        {
            string test = "My test string";
            Console.WriteLine("Test is string {0}", test is string);

            Person Frank = new Person();
            Frank.Name = "Frank Jon";
            Console.WriteLine("Frank is Person: {0}", Frank is Person);

            Child Mikky = new Child();
            Console.WriteLine("Child is Person {0}", Mikky is Person);

            Flower rose = new Flower();
            Console.WriteLine("Is Flower a Person {0}", rose is Person);

            Console.ReadKey();
        }
    }
}
