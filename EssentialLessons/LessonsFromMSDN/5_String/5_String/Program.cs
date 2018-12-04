using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class MyString
{
    private string mystring;

    public MyString(string mystring)
    {
        this.mystring = mystring;
    }
    public void Add(string newString)
    {
        mystring += " " + newString;
    }
    public string ShowString()
    {
        return mystring;
    }
    public string DeleteWord(string word)
    {
        if (mystring.IndexOf(word)!=-1)
        {
            mystring = mystring.Remove(mystring.IndexOf(word), word.Length);
        }

        return mystring;
    }
}
namespace _5_String
{
    class Program
    {
        static void Main(string[] args)
        {
            MyString testString = new MyString("Hello");

            testString.Add("my");
            testString.Add("Friend!");
            Console.WriteLine(testString.ShowString());

            testString.DeleteWord("my");
            Console.WriteLine(testString.ShowString());

            testString.DeleteWord("XXX");
            Console.WriteLine(testString.ShowString());

            Console.ReadKey();
        }
    }
}
