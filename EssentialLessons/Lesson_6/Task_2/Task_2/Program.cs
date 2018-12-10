using System;

/*
Создать статический класс FindAndReplaceManager
с методом void FindNext(string str) для поиска по книге из примера урока 005_Delegation.
При вызове этого метода, производится последовательный поиск строки в книге
*/

class Book
{
    public void FindNext(string str)
    {
        Console.WriteLine("Find text: {0}", str);
    }
}
static class FindAndReplace
{
    public static void FindNext(string str)
    {
        Book b = new Book();
        b.FindNext(str);
    }
}
namespace Task_2
{
    class Program
    {
        static void Main(string[] args)
        {
            FindAndReplace.FindNext("book");
            Console.ReadKey();

        }
    }
}
