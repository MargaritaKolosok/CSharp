using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinNMax
{
    public static class CheckMinMax
    {
        public static int[] MinMax(this int[] arr)
        {
            int min = arr[0], max = arr[0];
            int[] result;
            for (int i = 0; i < arr.Length; i++)
            {
                if (min > arr[i])
                {
                    min = arr[i];
                }
                if (max < arr[i])
                {
                    max = arr[i];
                }
            }
            result = new int[2] { min, max };

            Console.WriteLine("Min = {0}, max = {1}", min, max);
            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int[] arr = new int[] { 3,2,-1,56,7,8,90,32,1,2,4,7};
            CheckMinMax.MinMax(arr);
            Console.WriteLine(CheckMinMax.MinMax(arr));
            Console.ReadKey();
        }
    }
}
