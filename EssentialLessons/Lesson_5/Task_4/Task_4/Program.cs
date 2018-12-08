using System;
/*



Написать программу, вывода на экран информацию о товаре
 *
 */
namespace Task_4
{
    class Program
    {
        static void Main(string[] args)
        {
            Store myStore = new Store(3);

            Console.WriteLine(myStore[1]);
            Console.WriteLine(myStore["dodo"]);

            Console.ReadKey();

        }
    }
}
