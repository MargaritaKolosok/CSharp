using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 Класс Дробное число со знаком (Fractions).
 Число должно быть представлено двумя полями: целая часть 
 - длинное целое со знаком, дробная часть - беззнаковое короткое целое. 
 Реализовать арифметические операции сложения, вычитания, умножения и операции сравнения.
 В функции main проверить эти методы. */

class Fractions
{
    double num;
    int whole;
    double part;
    public Fractions(double num)
    {
        this.num = num;        
    }
    
    public double GetPart()
    {
        part = (double)num - Math.Truncate(num);
        return part;
    }
    public int GetWhole()
    {
        whole = (int)((double)num - part);
        return whole;
    }
}
namespace _11_Fractions
{
    class Program
    {
        static void Main(string[] args)
        {
            Fractions myF = new Fractions(107.6372);
            Console.WriteLine(myF.GetWhole());
            Console.WriteLine(myF.GetPart());
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
