using System;

/*
Создать статический класс FindAndReplaceManager
с методом void FindNext(string str) для поиска по книге из примера урока 005_Delegation.
При вызове этого метода, производится последовательный поиск строки в книге
*/

class Book
{
    string book = "";
    public Book(string str)
    {
        if (book!="")
        {
            book +=" /n" + str;
        }
        book = str;
    }
}
static class FindAndReplaceManager
{
    public void FindNext(string str)
    {

    }
}
namespace Task_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
