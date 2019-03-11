using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace join
{
    class Player
    {
        public string Name { get; set; }
        public string Team { get; set; }
    }
    class Team
    {
        public string Name { get; set; }
        public string Country { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            List<Team> teams = new List<Team>()
            {
                new Team { Name = "Бавария", Country ="Германия" },
                new Team { Name = "Барселона", Country ="Испания" },
                new Team { Name = "Ювентус", Country ="Италия" }
            };
            List<Player> players = new List<Player>()
            {
                new Player {Name="Месси", Team="Барселона"},
                new Player {Name="Неймар", Team="Барселона"},
                new Player {Name="Роббен", Team="Бавария"}
            };

            var result = players.Join(teams,
                p => p.Team,
                t => t.Name,
                (p,t) => new { Name = p.Name, Team = p.Team, Country = t.Country}
                );

            foreach (var player in result )
            {
                Console.WriteLine("{0}, {1}, {2}",player.Name, player.Team, player.Country);
            }

            var result2 = players.Zip(teams,
                (player, team) => new
                {
                    Name = player.Name,
                    Team = team.Name, Country = team.Country
                }               
                );
            foreach (var player in result2)
            {
                Console.WriteLine();
                Console.WriteLine("{0} - {1} ({2})", player.Name, player.Team, player.Country);
                
            }
            Console.ReadKey();
        }
    }
}
