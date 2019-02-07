using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Итак, украинский ИНН (индивидуальный налоговый номер) состоит из 10 цифр. В нём зашифрованы:

дата рождения (первые пять цифр образуют число, равное количеству дней от 01.01.1900 до даты рождения владельца ИНН);
пол (чётность девятой цифры: четная цифра – женский, нечетная – мужской);
checksum, т.е. контрольное число (последняя цифра) 

    Расшифровка ИИН :
первые 6 разрядов - это дата рождения ггммдд, то есть 12 августа 1985 года в ИИНе будет 850812
7 разряд отвечает за век рождения и пол. Если цифра нечетная - пол мужской, четная - женский. 1,2 - девятнадцатый век, 3,4 - двадцатый, 5,6 - двадцать первый.
8-11 разряды - заполняет орган Юстиции.
12 разряд - контрольная цифра, которая расчитывается по определенному алгоритму
 */
namespace GetINN
{
    class INN
    {
        DateTime birth;

        public INN(DateTime birth)
        {
            this.birth = birth;
        }
        public double Days()
        {
            DateTime startDay = new DateTime(1900,1,1);
            return (birth - startDay).TotalDays +1;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            DateTime birth = new DateTime(1985, 08, 12);
            INN my = new INN(birth);
            Console.WriteLine(my.Days());
            
            Console.ReadKey();
        }
    }
}
