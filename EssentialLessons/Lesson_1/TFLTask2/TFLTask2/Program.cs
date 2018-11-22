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
    public Book(string BookTitle, string BookAutor, string BookContent)
    {
       
    }
    class Title
    {
        public string BookTitle;
        public void Show()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(BookTitle);
        }
    }
    class Autor
    {
        public string BookAutor;
        public void Show()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(BookAutor);
        }
    }

    class Content
    {
        public string BookContent;
        public void Show()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(BookContent);
        }
    }
}

namespace TFLTask2
{
    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
