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
            for (int i=0; i<numbers.Length; i++)
            {
                for (int j = 0; j < numbers.Length; j++)
                {
                    if (numbers[i] + numbers[j] == target)
                    {
                        return new int[] { i, j};
                    }
                }
            }

            return new int[0];
        }

        static void Main(string[] args)
        {
            int[] myArray = new int[] { 1,2,3,4,5,6,7,8 };
            int[] result = TwoSum(myArray, 9);
            for(int i=0; i<result.Length; i++)
            {
                Console.WriteLine("Index " + result[i]);
            }
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
