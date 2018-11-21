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

    public Book(Author Author, Title Title)
    {
        this.myAuthor = Author;
        this.myTitle = Title;
      
    }
    public void Show()
    {
        myAuthor.Show();
        myTitle.Show();
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
//class Content
//{

//}

namespace LFRCTask3
{
    class Program
    {
        static void Main(string[] args)
        {
            Author newAuthor = new Author("Gilbert");
            Title newTitle = new Title("C#");
            Book myBook = new Book(newAuthor, newTitle);
            myBook.Show();
            Console.ReadKey();
        }
    }
}
