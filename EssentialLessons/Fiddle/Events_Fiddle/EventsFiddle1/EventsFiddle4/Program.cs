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

        event Contact contactAdded;
        event Contact deleteContact;

        public void AddToList(string str)
        {
            Book.Add(str);
            contactAdded?.Invoke($"Contact {str} was added to the list");
        }

        public void DeleteFromList(string str)
        {            
                if (Book.Contains(str))
                {                    
                    Book.Remove(str);
                    deleteContact?.Invoke($"Contact {str} deleted from the list");
                }
                else
                {
                    deleteContact?.Invoke($"Contact {str} does not exist in the list");
                }            
        }       

        event Contact ContactAdded
        {
            add
            {
                contactAdded += value;
            }  
            remove
            {
                contactAdded -= value;
            }
        }

        event Contact ContactDeleted
        {
            add
            {
                deleteContact += value;
            }
            remove
            {
                deleteContact -= value;
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

            Console.ReadKey();
        }
    }
}
