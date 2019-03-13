using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace group
{
    class Phone
    {
        public string Name { get; set; }
        public string Company { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Phone> phones = new List<Phone>
            {
                new Phone {Name="Lumia 430", Company="Microsoft" },
                new Phone {Name="Mi 5", Company="Xiaomi" },
                new Phone {Name="LG G 3", Company="LG" },
                new Phone {Name="iPhone 5", Company="Apple" },
                new Phone {Name="Lumia 930", Company="Microsoft" },
                new Phone {Name="iPhone 6", Company="Apple" },
                new Phone {Name="Lumia 630", Company="Microsoft" },
                new Phone {Name="LG G 4", Company="LG" }
            };

            var phoneGroup = phones.GroupBy(p => p.Company).Select(g => new { Name = g.Key, Count = g.Count(), Phones = g.Select(p=>p)});

            foreach (var p in phoneGroup)
            {
                Console.WriteLine(p.Name + " - " + p.Count);
                
            }
            Console.ReadKey();
        }
    }
}
