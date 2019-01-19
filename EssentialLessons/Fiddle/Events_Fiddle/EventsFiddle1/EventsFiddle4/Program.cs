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
        public void AddToList(string str)
        {
            Book.Add(str);
        }
        event Contact contactAdded;
        event Contact deleteContact;

        public event Contact ContactAdded
        {
            add
            {
                contactAdded += value;
            }  
            remove
            {

            }
        }

        public event Contact ContactDeleted
        {
            add { }
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
