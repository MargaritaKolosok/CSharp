using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Filtering2
{
    class User
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public List<string> Languages { get; set; }
        public User()
        {
            Languages = new List<string>();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            List<User> users = new List<User>
                {
                    new User {Name="Том", Age=23, Languages = new List<string> {"английский", "немецкий" }},
                    new User {Name="Боб", Age=27, Languages = new List<string> {"английский", "французский" }},
                    new User {Name="Джон", Age=29, Languages = new List<string> {"английский", "испанский" }},
                    new User {Name="Элис", Age=24, Languages = new List<string> {"испанский", "немецкий" }}
                };

            var selectedUsers = users.SelectMany(u => u.Languages,
                (u, l) => new { User = u, Lang = l })
                .Where(u => u.Lang == "английский" && u.User.Age > 25)
                .Select(u => u.User);

            foreach (var u in selectedUsers)
            {
                Console.WriteLine(u.Name + " is " + u.Age + " " + u.Languages.ToString());
            }
            Console.ReadKey();
        }
    }
}
