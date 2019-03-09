using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sorting
{
    class User
    {
        public string Name;
        public int Age;
    }
    class Program
    {
        static void Main(string[] args)
        {
            List<User> users = new List<User>()
            {
                new User { Name = "Tom", Age = 33 },
                new User { Name = "Bob", Age = 30 },
                new User { Name = "Tom", Age = 21 },
                new User { Name = "Sam", Age = 43 }
            };

            var sortedUsers = users.OrderBy(u => u.Age);
            var sortedNames = users.OrderByDescending(u => u.Name);
            var sortedAll = users.OrderBy(u => u.Name).ThenBy(u => u.Age);

            foreach (var user in sortedUsers)
            {
                Console.WriteLine(user.Age);
            }

            foreach (var user in sortedNames)
            {
                Console.WriteLine(user.Name);
            }

            foreach (var user in sortedAll)
            {
                Console.WriteLine(user.Name + " " + user.Age);
            }

            Console.ReadKey();
        }
    }
}
