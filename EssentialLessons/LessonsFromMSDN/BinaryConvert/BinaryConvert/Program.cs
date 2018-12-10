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
    public string BinaryNum
    {
       get { return binary; }
    }

    static public bool IsBinary(string str)
    {
        int ok = 0;
        for (int i=0; i<str.Length; i++)
        {
            if (int.TryParse(str[i].ToString(), out int result))
            {
                if (result==1 || result==0) { ok++; }
            }
            else { return false; }

        }
        if (ok == str.Length)
        {
            return true;
        }
        else { return false; }
    }

    public int ConvertToInt()
    {
        int result;
        if (IsBinary(binary))
        {
            result = Convert.ToInt32(binary, 2);
        }
        else { result = 0; }
        return result;
    }
}
namespace BinaryConvert
{
    class Program
    {
        static void Main(string[] args)
        {
            
            Console.WriteLine(Binary.IsBinary("10903940"));

            
            Console.WriteLine(Binary.IsBinary("101010101111"));
            Console.WriteLine(Binary.IsBinary("101010ksjd1111"));
            Binary b = new Binary("101001010101");
            Console.WriteLine(b.ConvertToInt());
            Console.ReadKey();
        }
    }
}
