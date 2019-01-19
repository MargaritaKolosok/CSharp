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
        List<string> Book;

        event Contact contactAdded;
        event Contact deleteContact;

        public void AddToList(string str)
        {
            Book.Add(str);
            contactAdded?.Invoke("$Contact {str} was added to the list");
        }

        public void DeleteFromList(string str)
        {
            foreach(string s in Book)
            {
                if (Book.Contains(str))
                {
                    Book.Remove(str);
                    deleteContact?.Invoke("$Contact {str} deleted from the list");
                }
                else
                {
                    deleteContact?.Invoke("$Contact {str} does not exist in the list");
                }
            }
        }
        

        public event Contact ContactAdded
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

        public event Contact ContactDeleted
        {
            add
            {
                deleteContact -= value;
            }
            remove
            {
                deleteContact -= value;
            }
        }
    }
    
    class Program
    {
        public static void ContactMethod(string str)
        {
            
        }
        static void Main(string[] args)
        {

        }
    }
}
