using System;
using System.Reflection;

namespace ConsoleSandBox
{
    class Program
    {
        public class Product
        {
            public string id { get; set; }
        }
        static void Main(string[] args)
        {
            // Get the Type and MemberInfo.
            Product p = new Product();
            Type t = Type.GetType("Product");
            //MemberInfo[] memberArray = t.GetMembers();

            // Get and display the type that declares the member.
            Console.WriteLine("There are {0} members in {1}",
                              memberArray.Length, t.FullName);

            foreach (var member in memberArray)
            {
                Console.WriteLine(">> Member {0} declared by {1}",
                                  member.Name, member.DeclaringType);
            }
        }
    }
}
