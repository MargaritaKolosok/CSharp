using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
struct Book
{
    public string Name;
    public int Pages;

    public Book(string s, int i)
    {
        Name = s;
        Pages = i;
    }
}
class Magazine
{
    public string Name;
    public int Pages;

    public Magazine(string s, int i)
    {
        Name = s;
        Pages = i;
    }
    public Magazine()
    {

    }
}
namespace Structures
{
    class Program
    {
        static void Main(string[] args)
        {
            Book book1 = new Book("Book1 name", 256);
            Book book2 = new Book();
            book2 = book1;

            Console.WriteLine(book1.Name + " " + book2.Name);
            book1.Name = "Book 1 name changed";
            Console.WriteLine(book1.Name + " " + book2.Name);

            Magazine mag1 = new Magazine("Magazine name", 256);
            Magazine mag2 = new Magazine();
            mag2 = mag1;

            Console.WriteLine(mag1.Name + " " + mag2.Name);
            mag1.Name = "Magazine1 name changed";
            Console.WriteLine(mag1.Name + " " + mag2.Name);
            Console.ReadKey();

        }
    }
}
