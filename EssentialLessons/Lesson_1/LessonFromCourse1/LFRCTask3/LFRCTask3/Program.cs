using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
Создать класс Book.
Создать классы Title, Author и Content, каждый из которых должен содержать
одно строковое поле и метод void Show().

Реализуйте возможность добавления в книгу названия книги, имени автора и содержания.
Выведите на экран разными цветами при помощи метода Show() название книги, имя автора и
содержание. 

*/

class Book
{
    public Author myAuthor;
    public Title myTitle;
    public Content myContent;

    public Book(Author Author, Title Title, Content Content)
    {
        this.myAuthor = Author;
        this.myTitle = Title;
        this.myContent = Content;
      
    }
    public void Show()
    {
        myAuthor.Show();
        myTitle.Show();
        myContent.Show();
    }
}

class Title
{
    private string bookTitle;
    public Title(string bookTitle)
        {
        this.bookTitle = bookTitle;
        }
     public void Show()
        {
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine(bookTitle);
        }
}

class Author
{
    private string authorName;
   
    public Author(string authorName) {
             this.authorName = authorName; 
    }

    public void Show()
    {
        Console.BackgroundColor = ConsoleColor.Blue;
        Console.WriteLine(authorName);
    }
    
   
}
class Content
{
    private string bookContent;
    public Content(string bookContent)
    {
        this.bookContent = bookContent;
    }

    public void Show()
    {
        Console.BackgroundColor = ConsoleColor.Cyan;
        Console.WriteLine(bookContent);
    }
}

namespace LFRCTask3
{
    class Program
    {
        static void Main(string[] args)
        {
            Author newAuthor = new Author("Gilbert");
            Title newTitle = new Title("C#");
            Content newContent = new Content("Создать класс Book.Создать классы Title, Author и Content, каждый из которых должен содержать одно строковое поле и метод void Show().");

            Book myBook = new Book(newAuthor, newTitle, newContent);
            myBook.Show();
            Console.ReadKey();
        }
    }
}
