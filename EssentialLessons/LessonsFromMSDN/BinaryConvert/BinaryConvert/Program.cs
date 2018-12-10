using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Дан массив символов, содержащий число в двоичной системе счисления.
Проверить правильность ввода этого числа (в его записи должны
быть только символы 0 и 1). Если число введено неверно, повторить
ввод. При правильном вводе перевести число в десятичную систему
счисления */
class Binary
{
    string binary;

    public Binary(string binary)
    {
        this.binary = binary;
    }   

    bool IsBinary(string str)
    {
        int ok = 0;
        for (int i=0; i<str.Length; i++)
        {
            if (int.TryParse(str[i].ToString(), out int result))
            {
                if (result==1 || result==0)
                {
                    ok++;
                }
            }

            else
            {
                return false;
            }
        }
            return  (ok == str.Length)?  true : false;            
    }

    public int ConvertToInt()
    {
        int result;
        if (IsBinary(binary))
        {
            result = Convert.ToInt32(binary, 2);
        }
        else
        {
            Console.WriteLine("Is not binary number");
            result = 0;
        }
        return result;
    }
}
namespace BinaryConvert
{
    class Program
    {
        static void Main(string[] args)
        {           
            Binary b = new Binary("101001010101");
            Console.WriteLine(b.ConvertToInt());
            Binary c = new Binary("10100djkhs0101");
            Console.WriteLine(c.ConvertToInt());
            Console.ReadKey();
        }
    }
}
