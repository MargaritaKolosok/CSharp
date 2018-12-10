using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class Book
{ 
  public  class Note
    {
        string note = "";

        public Note(string str)
        {
            note += str;
        }
        public string Text
        {
            get;
            set;
        }
    }

    public void FindText(string str)
    {
        Console.WriteLine(str + "/n");
    }
}
namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
        {
            Book b = new Book();           
            Book.Note v = new Book.Note("Book note");

            v.Text = "Text in note subclass";
            b.FindText(v.Text);
            Console.ReadKey();
        }
    }
}
