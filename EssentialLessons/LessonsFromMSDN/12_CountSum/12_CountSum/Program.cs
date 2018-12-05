using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace _12_CountSum
{

    class Program
    {
        static int CountSum(int[] arr, out int min, out int max)
        {
            int sum = 0;
            min = arr[0]++;
            max = arr[0]++;
            for (int i = 0; i < arr.Length; i++)
            {               
                sum += arr[i];
                if (arr[i] < min)
                {
                    min = arr[i];
                }
                if (arr[i]> max)
                {
                    max = arr[i];
                }
                    
            }
            return sum;
        }

        static void Main(string[] args)
        {
            int[] arr = {1,2,3,4,5,6,7,8,9,88,676 };
            Console.WriteLine("Sum of array is {0}", CountSum(arr, out int a, out int b));
            Console.WriteLine("Max is {0}, min is {1}", b, a);
            Console.ReadKey();
           
        }
    }
}
