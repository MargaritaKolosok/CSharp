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
                    price = value;
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
            this.bookName = bookName;
            this.pages = pages;
            this.price = price;
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
            Console.WriteLine("Book Name is {0}", bookName);
            Console.WriteLine("Number of pages {0}", pages);
            Console.WriteLine("Price: {0}", price);
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            Book myBook = new Book();
           
            Console.WriteLine(myBook.PagePrice());

            Book myBook2 = new Book("Программирование C#", 200, 700.50);
            Console.WriteLine(myBook2.PagePrice());

            Console.ReadKey();
        }
    }
}
