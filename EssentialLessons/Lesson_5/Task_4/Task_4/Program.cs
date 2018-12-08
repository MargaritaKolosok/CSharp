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
            Article ar1 = new Article("coffee", "coffeshop", 18.6);
            Article ar2 = new Article("tea", "coffeshop", 10);
            Article ar3 = new Article("suger", "ATB", 3);
            myStore.AddArticle(ar1, 0);
            myStore.AddArticle(ar2, 1);
            myStore.AddArticle(ar3, 2);

            Console.WriteLine(myStore[1]);
            Console.WriteLine(myStore["coffee"]);
            Console.WriteLine(myStore["x"]);


            Console.ReadKey();

        }
    }
}
