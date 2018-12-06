using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MinVal
{
    int[] arr;
    public MinVal(params int[] num)
    {
        arr = num;
    }
    public int Min()
    {
        if (arr.Length == 0)
        {
            Console.WriteLine("No elements in array");
            return 0;
        }
        int min = arr[0];
        for (int i=1; i<arr.Length; i++)
        {
            if (min>arr[i])
            {
                min = arr[i];
            }
        }
        return min;
    }
}

namespace _13_MinC_ValParams
{
    class Program
    {
        static void Main(string[] args)
        {
            MinVal min1 = new MinVal(1, 5, 6, 7, 8, 9, -2);
            Console.WriteLine(min1.Min());
            MinVal min2 = new MinVal();
            Console.WriteLine(min2.Min());

            Console.ReadKey();
        }
    }
}
