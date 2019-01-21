using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsFiddle4
{
    public delegate void Contact(string contact);

    class ContactBook
    {
        List<string> Book = new List<string>();

        event Contact ContactAdded;
        event Contact ContactDeleted;

        public void AddToList(string str)
        {
            Book.Add(str);
            ContactAdded?.Invoke($"Contact {str} was added to the list");
        }

        public void DeleteFromList(string str)
        {            
                if (Book.Contains(str))
                {                    
                    Book.Remove(str);
                    ContactDeleted?.Invoke($"Contact {str} deleted from the list");
                }
                else
                {
                    ContactDeleted?.Invoke($"Contact {str} does not exist in the list");
                }            
        }    
                
        public ContactBook()
        {
            ContactAdded += ContactMethod;
            ContactDeleted += ContactMethod2;
        }

        static void ContactMethod(string str)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
        }

        static void ContactMethod2(string str)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(str);
        }

        public void ShowAll()
        {
            foreach (string str in Book)
            {
                Console.WriteLine(str);
            }
        }
    }

    class Program
    {              
        static void Main(string[] args)
        {
            ContactBook book = new ContactBook();            

            book.AddToList("Greta");
            book.AddToList("John");
            book.AddToList("Marta");
            book.AddToList("Sam");

            book.DeleteFromList("Sam");
            book.DeleteFromList("Sammy");

            book.ShowAll();

            Console.ReadKey();
        }
    }
}
