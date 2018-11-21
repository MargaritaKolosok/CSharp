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

    public Book(Author Author)
    {
        this.myAuthor = Author;
      
    }
    public void Show()
    {
        myAuthor.Show();
    }
}

//class Title
//{

//}

class Author
{
    private string authorName;
   
    public Author(string authorName) {
             this.authorName = authorName; 
    }

    public void Show()
    {
        Console.BackgroundColor = ConsoleColor.Red;
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
            Book myBook = new Book(newAuthor);
            myBook.Show();
            Console.ReadKey();
        }
    }
}
