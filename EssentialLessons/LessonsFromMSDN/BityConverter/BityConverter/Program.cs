using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 
килобайт	КБайт (KБ)	1024 байта
мегабит	мбит (мб)	1 000 килобит
мегабайт	МБайт (МБ)	1024 килобайта
гигабит	гбит (гб)	1 000 мегабит
гигабайт	ГБайт (ГБ)	1024 мегабайта
терабит	тбит (тб)	1 000 гигабит
терабайт	ТБайт (ТБ)	1024 гигабайта
     */
class Bite
{
    private long bite;
    public long BiteNum
    {
        set => bite = value;
        get => bite;
    }
    public Bite(long bite)
    {
        BiteNum = bite;
    }
    public void ConvertToKB()
    {
        Console.WriteLine("B {0} in KB {1}", BiteNum, BiteNum / 1024);
    }

   
   
}

namespace BityConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            Bite muBite = new Bite(1024);
            muBite.ConvertToKB();
            Console.ReadLine();
        }
    }
}
