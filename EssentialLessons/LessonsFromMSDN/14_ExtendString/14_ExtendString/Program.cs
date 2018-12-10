using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
static class String
{
    public static string Revert(this string str)
    {
        string result = "";
        int length = str.Length - 1;
        foreach (char ch in str)
        {
            result += str[length];
            length--;
        }
        return result;
    }

    public static string NumbersInString(this string str)
    {
        string result = "";
        foreach (char ch in str)
        {
            if (int.TryParse(ch.ToString(), out int num))
            {
                result += num;
            }
        }
        return result;
    }
}
namespace _14_ExtendString
{
    class Program
    {
        static void Main(string[] args)
        {
            string test = "Hello my dear frieds";
            Console.WriteLine(test.Revert());

            string test2 = "Hello1 my2 dear3 frieds777";
            Console.WriteLine(test2.NumbersInString());

            Console.ReadKey();
        }
    }
}
