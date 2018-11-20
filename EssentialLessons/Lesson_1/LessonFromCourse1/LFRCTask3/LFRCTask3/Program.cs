using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Создать класс Book. Создать классы Title, Author и Content, каждый из которых должен содержать
одно строковое поле и метод void Show().
Реализуйте возможность добавления в книгу названия книги, имени автора и содержания.
Выведите на экран разными цветами при помощи метода Show() название книги, имя автора и
содержание. 
*/

class Book
{
    public Book(string Author)
    {
        Author BookAuthor = new Author();
        BookAuthor.SetAuthorName = Author;
        BookAuthor.Show();

    }
}

//class Title
//{

//}

class Author
{
    public string BookAuthor;
    public string SetAuthorName
{
        set { BookAuthor = value; }
}
    public void Show()
    {
        Console.WriteLine(BookAuthor);
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
            Book myBook = new Book("Gilbert Shild");
            Console.ReadKey();
        }
    }
}
