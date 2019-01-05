using System;

namespace ConsoleApp1
{
    class Book
    {
        string bookName;
        int pages;
        double price;

        public string BookName
        {
            get { return bookName; }
            private set { bookName = value; }
        }
        public int Pages
        {
            get { return pages; }
            private set
            {
                if (value > 0)
                {
                    pages = value;
                }
                else
                {
                    pages = 0;
                    Console.WriteLine("Unknown number of pages");
                }
            }
        }

        public double Price
        {
            get { return price; }
            private set
            {
                if (value > 0)
                {
                    if (BookName.Contains("Программирование"))
                    {
                        price = value * 2;
                    }
                    else
                    {
                        price = value;
                    }                       
                }
                else
                {
                    price = 0;
                    Console.WriteLine("Price can't be negative. Unknown price");
                }
            }
        }

        public Book()
        {
            bookName = "";
            pages = 1;
            price = 1;
        }
        public Book(string bookName, int pages, double price)
        {
            BookName = bookName;
            Pages = pages;
            Price = price;
        }
        public double PagePrice()
        {
            if (BookName.Contains("Программирование"))
            {
                return (price / pages) * 2;
            }
            else
            {
                return price / pages;
            }
            
        }

        public void ShowInfo()
        {
            Console.WriteLine("Book Name is {0}", BookName);
            Console.WriteLine("Number of pages {0}", Pages);
            Console.WriteLine("Price: {0}", Price);
            Console.WriteLine("Page Price: {0}", PagePrice());
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Book myBook = new Book();
            myBook.ShowInfo();
            Console.WriteLine();
            Book myBook2 = new Book("Программирование C#", 200, 740);
            myBook2.ShowInfo();
            Console.WriteLine((myBook2.BookName).Contains("Программирование"));

            Console.ReadKey();
        }
    }
}
