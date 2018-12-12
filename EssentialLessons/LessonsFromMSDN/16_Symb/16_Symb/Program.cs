using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Введите массив символов из 10 элементов. Замените символы-цифры на
символ ' * ' 

 */
class Sequrity
{
    public static string[] Secure(string[] arr)
    {
        string[] secure = new string[arr.Length];
        for (int i=0; i<arr.Length; i++)
        {
            if (int.TryParse(Convert.ToString(arr[i]), out int result))
            {
                secure[i] = "#";
            }
            else
            {
                secure[i] = arr[i];
            }
        }
       
        return secure;
    }
}


namespace _16_Symb
{
 
    class Program
    {
        static void Main(string[] args)
        {
            string[] my = new string[];
        }
    }
}
