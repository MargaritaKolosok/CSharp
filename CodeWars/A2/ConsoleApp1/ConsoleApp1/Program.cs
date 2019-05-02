using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        public static int[] TwoSum(int[] numbers, int target)
        {
            int[] ResultArray = new int[2];
            int num = 0;

            for (int i=0; i<numbers.Length; i++)
            {
                for (int j = 0; j < numbers.Length; j++)
                {
                    if (i!=j)
                    {
                        if (numbers[i] + numbers[j] == target)
                        {
                            ResultArray[0] = numbers[i];
                           // num++;
                            ResultArray[1] = numbers[j];
                        }
                    }
                }
            }

            return ResultArray;
        }

        static void Main(string[] args)
        {
            int[] myArray = new int[] { 1,2,3,4,5,6,7,8 };
            int[] result = TwoSum(myArray, 9);
            for(int i=0; i<result.Length; i++)
            {
                Console.WriteLine(result[i]);
            }
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
