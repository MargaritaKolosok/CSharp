using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class ArrayCreator
{
    private int[] arr;
    public ArrayCreator(int numOfEl)
    {
        arr = new int[numOfEl];
    }

    public void AddRandomElements()
    {
        Random random = new Random();
        for (int i=0; i<arr.Length; i++)
        {            
            arr[i] = random.Next(1,100);
        }
    }

    private int Average()
    {
        int sum = 0;
        for (int i=0; i<arr.Length; i++)
        {
            sum += arr[i];
        }
        return sum / arr.Length;
    }

    public void Show()
    {
        for (int i = 0; i < arr.Length; i++)
        {           
            Console.WriteLine(arr[i]);
        }

        Console.WriteLine("Average num in array: {0}", Average());
    }
}
namespace _6_Array_task_1
{
    class Program
    {
        static void Main(string[] args)
        {
            ArrayCreator myArray = new ArrayCreator(6);
            myArray.AddRandomElements();
           
            myArray.Show();
            Console.ReadKey();
        }
    }
}
