using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
Создать класс Converter.
В теле класса создать пользовательский конструктор, который принимает три вещественных аргумента,
и инициализирует поля соответствующие курсу 3-х основных валют, по отношению к гривне – public
Converter(double usd, double eur, double rub).
Написать программу, которая будет выполнять конвертацию из гривны в одну из указанных валют,
также программа должна производить конвертацию из указанных валют в гривну. 
 * */
class Converter
{
    private double usdRate;
    private double eurRate;
    private double rubRate;

    public Converter(double usd, double eur, double rub)
    {
        this.usdRate = usd;
        this.eurRate = eur;
        this.rubRate = rub;
    }
    public double ConvertToUsd(double uah)
    {
        return uah / usdRate;
    }
    public double ConvertToEur(double uah)
    {
        return uah / eurRate;
    }
    public double ConvertToRub(double uah)
    {
        return uah / rubRate;
    }
    public double ConvertFromUsd(double usd)
    {
        return usd / usdRate;
    }
    public double ConvertFromEur(double eur)
    {
        return eur / eurRate;
    }
    public double ConvertFromRub(double rub)
    {
        return rub / rubRate;
    }
}


namespace ConverterExc
{
    class Program
    {
        static void Main(string[] args)
        {
            Converter myExchange = new Converter(27.98, 32.66, 2.37);
            
            Console.WriteLine(myExchange.ConvertToUsd(200));
            Console.ReadKey();


        }
    }
}
