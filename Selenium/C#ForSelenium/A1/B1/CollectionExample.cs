using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B1
{
    class User
    {
        public string Name;
        public int Age;
        public string Email;
        public Int64 PhoneNumber;
    }

    class CollectionExample
    {
        public static void GenericCollectionWithCustomType()
        {
            List<User> users = new List<User>();

            users.Add(new User { Name = "Rita", Age = 26 });
            users.Add(new User { Name = "Rita", Age = 26 });
            users.Add(new User { Name = "Rita", Age = 26 });

            var usersList = from user in users
                            select user.Name;

            foreach (var user in usersList)
            {
                Console.WriteLine(user);
            }
        }
    }
}
