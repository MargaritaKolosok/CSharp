using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * 
килобайт	КБайт (KБ)	1024 байта

мегабайт	МБайт (МБ)	1024 килобайта

гигабайт	ГБайт (ГБ)	1024 мегабайта

терабайт	ТБайт (ТБ)	1024 гигабайта
     */
class Bite
{
   // private long bite;
    public double bite
    {
        set;
        get;
    }
    public Bite(long bite)
    {
        this.bite = bite;
    }
    public void ConvertToKB()
    {
        Console.WriteLine("Bite {0} in KB {1}", bite, bite / 1024);
    }
    public void ConvertToMB()
    {
        Console.WriteLine("Bite {0} in KB {1}", bite, (bite / 1024) / 1024);
    }
    public void ConvertToGB()
    {
        Console.WriteLine("Bite {0} in KB {1}", bite, ((bite / 1024) / 1024)/ 1024);
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
            muBite.ConvertToMB();
            muBite.ConvertToGB();
            Console.ReadLine();
        }
    }
}
