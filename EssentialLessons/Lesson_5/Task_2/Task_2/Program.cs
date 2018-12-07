using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 Требуется:
Создать массив размерностью N элементов, заполнить его произвольными целыми значениями.
Вывести наибольшее значение массива, наименьшее значение массива, общую сумму элементов,
среднее арифметическое всех элементов, вывести все нечетные значения. 
*/

class myArr
{
    private int[] arr;
    Random random = new Random();

    public myArr(int x)
    {
      arr = new int[x];
        
        for (int i=0; i<arr.Length; i++)
        {
            arr[i] = (int)random.Next(1,10);
        }     
    }

    int max, min, sum, average;
    public void Max()
    {
        max = arr[0];
        for(int i=0; i<arr.Length; i++)
        {
            if (arr[i] > max)
                max = arr[i];
        }
        Console.WriteLine("Max value is {0}", max);
    }
    public void ShowElemets()
    {
        foreach (int x in arr)
        {
            Console.WriteLine("Elemets array index {0} = {1}", x, arr[x]);
        }
    }
    public void Min()
    {
        min = arr[0];
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] <= min)
            {
                min = arr[i];
            }
                
        }
        Console.WriteLine("Min value is {0}", min);
    }
    public void Avr(out int sum)
    {
        sum = 0;
        for (int i = 0; i < arr.Length; i++)
        {
            sum += arr[i];
        }
        Console.WriteLine("Avarage value is {0}", sum / arr.Length);
    }


}
namespace Task_2
{
    class Program
    {
        static void Main(string[] args)
        {
            myArr my = new myArr(10);
            
            my.ShowElemets();
            my.Avr(out int summa);
            my.Max();
            my.Min();
            Console.WriteLine("Summa {0}", summa);
                
            Console.ReadKey();
        }
    }
}
