using System;
using System.Collections.Generic;

namespace A1
{   
    class Program
    {
        public static void GenericCollection()
        {
            string[] user1 = new string[] { "MyName", "MyCountry", "Age" };
            string[] user2 = new string[] { "MyName", "MyCountry", "Age" };
            string[] user3 = new string[] { "MyName", "MyCountry", "Age" };

            Dictionary<int, string[]> dict = new Dictionary<int, string[]>();
            dict.Add(1, user1);
            dict.Add(2, user2);
            dict.Add(3, user3);

            foreach (var value in dict)
            {
                string[] usersInfo = value.Value;

                foreach (var user in usersInfo)
                {
                    Console.WriteLine(user);
                }
            }
        }

        public static void GenericCollectionWithCustomType()
        {
            List<User> userList = new List<User>();
            userList.Add(new User { Name = "Rita", Age = 26 });
            userList.Add(new User { Name = "Rita", Age = 26 });
            userList.Add(new User { Name = "Rita", Age = 26 });
            foreach (var value in userList)
            {
                Console.WriteLine(value.Name + " " + value.Age);
            }
        }

        class User
        {
            public string Name;
            public int Age;
            public string Email;
            public Int64 PhoneNumber;
        }



        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            GenericCollection();
            GenericCollectionWithCustomType();
            

            Console.ReadKey();
        }
    }
}
