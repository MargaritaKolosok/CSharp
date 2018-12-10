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
}
namespace _14_ExtendString
{
    class Program
    {
        static void Main(string[] args)
        {
            string test = "Hello my dear frieds";
            Console.WriteLine(test.Revert());
            
            Console.ReadKey();
        }
    }
}
