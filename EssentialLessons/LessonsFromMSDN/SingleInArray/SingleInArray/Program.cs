using System;
/*
You are given an odd-length array of integers, in which all of them are the same, except for one single number.

Complete the method which accepts such an array, and returns that single different number.

The input array will always be valid! (odd-length >= 3)

Examples
[1, 1, 2] ==> 2
[17, 17, 3, 17, 17, 17, 17] ==> 3
 * */
namespace SingleInArray
{
  class Program
   {
        public static int Stray(int[] array)
        {
            int unic = array[0];

            for (int i=0; i < array.Length; i++)
            {
                for (int j=0; j<array.Length; j++)
                {
                    if (i==j)
                    {
                        continue;
                    }

                    if (array[i] == array[j])
                    {
                        break;
                    }

                    else
                    {
                        unic = array[i];
                    }
                }
            }

            return unic;
        }
            
        
        static void Main(string[] args)
        {
            int[] test = new int[] { 1,2,1,1,1,1,1,1,1};
            int result = Stray(test);
            Console.WriteLine(result);

            Console.ReadKey();
        }
    }
}
