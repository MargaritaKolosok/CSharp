using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace _12_CountSum
{

    class Program
    {
        static int CountSum(int[] arr)
        {
            int sum = 0;            
            for (int i = 0; i < arr.Length; i++)
            {               
                sum += arr[i];
            }
            return sum;
        }

        static void Main(string[] args)
        {
            int[] arr = {1,2,3,4,5,6,7,8,9,88,676 };
            Console.WriteLine(CountSum(arr));
            Console.ReadKey();
           
        }
    }
}
