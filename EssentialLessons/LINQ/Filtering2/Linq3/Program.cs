using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linq3
{
    class User
    {
        public string Name { get; set; }
        public int Age { get;  set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<User> users = new List<User>()
            {
                new User { Name = "Sam", Age = 43 },
                new User { Name = "Tom", Age = 33 }
            };
            var people = from u in users
                         let name = "Mr. " + u.Name
                         select new
                         {
                             Name = name,
                             Age = u.Age
                         };

            foreach (var person in people)
            {
                Console.WriteLine(person.Age);
            }
        }
    }
}
