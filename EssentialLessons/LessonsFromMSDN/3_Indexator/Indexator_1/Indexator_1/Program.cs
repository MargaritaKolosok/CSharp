using System;

namespace Indexator_1
{
    class Actor
    {
        string[] Roles = { "Mother", "Doughter", "Witch", "Queen" };

        public string this[int x]
        {
            get
            {
                if (x >= 0 && x < Roles.Length)
                {
                    return Roles[x];
                }
                else
                {
                    return string.Format("No such role");
                }
            }
        }
            
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("");
            Actor actor = new Actor();
            Console.WriteLine(actor[1]);
            Console.WriteLine(actor[3]);
            Console.WriteLine(actor[6]);
            Console.ReadKey();
        }
    }
}
