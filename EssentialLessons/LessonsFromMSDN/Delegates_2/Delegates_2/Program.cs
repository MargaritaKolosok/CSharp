using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Напишите программу, в которой объявляется делегат для методов с
двумя аргументами (символ и текст) и целочисленным результатом. В
главном классе необходимо описать два статических метода. Один
статический метод результатом возвращает количество вхождений символа
(первый аргумент) в текстовую строку (второй аргумент). Другой метод
результатом возвращает индекс первого вхождения символа (первый
аргумент) в текстовую строку (второй аргумент), или значение -1, если
символ в текстовой строке не встречается. В главном методе создать
экземпляр делегата и с помощью этого экземпляра вызвать каждый из
статических методов.
 */

namespace Delegates_2
{
    public delegate int MyDelegate(char ch, string str);

    class Program
    {
        public static int NumOfSumbols(char ch, string str)
        {
            int num = 0;
            foreach (char x in str)
            {
                if (x == ch)
                {
                    num++;
                }                
            }
            return num;
        }
        public static int IndexofSumbol(char ch, string str)
        {
            return str.IndexOf(ch);

        }

        static void Main(string[] args)
        {
            MyDelegate my = NumOfSumbols;
            MyDelegate my2 = IndexofSumbol;

            Console.WriteLine(my2('H', "Hello world"));
            Console.WriteLine(my('l', "Hello world"));
            Console.ReadKey();
        }
    }
}
