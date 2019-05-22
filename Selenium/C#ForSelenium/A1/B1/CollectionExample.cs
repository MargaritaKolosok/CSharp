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
            users.Add(new User { Name = "Maria", Age = 32 });
            users.Add(new User { Name = "Jane", Age = 29 });

            //var usersList = from user in users
            //                where user.Age > 26
            //                select user.Name;            

            var usersList = users.Where(x => x.Age > 30).Select(x => x);

            foreach (var user in usersList)
            {                
                Console.WriteLine(user.Name + " " + user.Age);
            }
        }
    }
}
