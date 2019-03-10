using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agragation
{ class User
    {
        public string Name;
        public int Age;
    }

    class Program
    {
        static void Main(string[] args)
        {
            int[] numbers = { 1, 2, 3, 4, 10, 34, 55, 66, 77, 88 };

            int size = numbers.Count(i => i%2 ==0 && i > 10);
            int sum = numbers.Sum();

            Console.WriteLine(size);
            Console.WriteLine(sum);

            List<User> users = new List<User>()
            {
                new User { Name = "Tom", Age = 23 },
                new User { Name = "Sam", Age = 43 },
                new User { Name = "Bill", Age = 35 }
            };

            int maxAge = users.Max(u => u.Age);
            int minAge = users.Min(u => u.Age);

            Console.WriteLine(maxAge);
            Console.WriteLine(minAge);

            Console.ReadKey();
        }
    }
}
