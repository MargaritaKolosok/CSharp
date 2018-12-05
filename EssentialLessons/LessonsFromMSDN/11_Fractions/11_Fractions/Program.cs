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
    
     double GetPart()
    {
        part = (double)num - Math.Truncate(num);
        return part;
    }
     int GetWhole()
    {
        whole = (int)((double)num - part);
        return whole;
    }
    public void ShowParts()
    {
        Console.WriteLine("Whole: {0}, Part {1}",  this.GetWhole(), this.GetPart());
    }
}
namespace _11_Fractions
{
    class Program
    {
        static void Main(string[] args)
        {
            Fractions myF = new Fractions(107.6372);            
            myF.ShowParts();            
            Console.ReadKey();
        }
    }
}
