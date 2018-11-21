using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Book : Title
{
  public  string Author;
  public string Title;
  public string Content;

    public Book(string Author, string Title, string Content)
    {
        this.Author = Author;
        this.Content = Content;
        this.Title = Title;
    }


}

class Title 
{
    private string BookTitle;
    
    public void Show()
    {
        Console.BackgroundColor = ConsoleColor.Blue;
        Console.WriteLine(BookTitle);
    }
    
}
class Author
{
    private string BookAuthor;
    public Author(value)
    {
        BookAuthor = value;
    }
    public void Show()
    {
        Console.BackgroundColor = ConsoleColor.Cyan;
        Console.WriteLine(BookAuthor);
    }
}
class Content
{
    private string BookContent;
    public void Show()
    {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine(BookContent);
    }
}

namespace Books
{
    class Program
    {
        static void Main(string[] args)
        {
            Author Autor1 = new Author("Gilbert Shild");

            Book myBook = new Book("Gilbert Shild", "C#", "How to code");
            myBook.Show();
            Console.ReadKey();
        }
    }
}
