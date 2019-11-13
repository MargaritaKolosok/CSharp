using System;

namespace A3
{
    class Program
    {
//You probably know the "like" system from Facebook and other pages.People can "like" blog posts, pictures or other items. We want to create the text that should be displayed next to such an item.

//Implement a function likes :: [String] -> String, which must take in input array, containing the names of people who like an item.It must return the display text as shown in the examples:

//Kata.Likes(new string[0]) => "no one likes this"
//Kata.Likes(new string[] {"Peter"}) => "Peter likes this"
//Kata.Likes(new string[] {"Jacob", "Alex"}) => "Jacob and Alex like this"
//Kata.Likes(new string[] {"Max", "John", "Mark"}) => "Max, John and Mark like this"
//Kata.Likes(new string[] {"Alex", "Jacob", "Mark", "Max"}) => "Alex, Jacob and 2 others like this"
        public static string Likes(string[] name)
        {
            if (name.Length != 0)
            {
                if(name.Length == 1)
                {
                    return name.ToString() + "like this";
                } 
                else if(name.Length < 2)
                {
                    string result;

                    foreach(string n in name)
                    {
                        result = n + ", ";
                    }                    
                }
            }
            else
            {
                if (name.Length == 0)
                {
                    return "0 likes";
                }
            }
            throw new NotImplementedException();
        }
        static void Main(string[] args)
        {
            string[] l = new string[] {};
            string[] l2 = new string[2] { "Alex", "Rita" };
            Console.WriteLine(Likes(l));
            Console.WriteLine(Likes(l2));
            Console.ReadLine();
        }
    }
}
