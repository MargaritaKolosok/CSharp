using System;

namespace SingleInArray
{
    class Program
    {
        public static int Stray(int[] array)
        {
            int one;
            one = array[0];
            for (int i=0; i< array.Length; i++)
            {
                
                for (int j=0; j<array.Length; j++)
                {
                    if (j != i)
                    {
                        if (one == array[j])
                        {
                            break;
                        }
                        else
                        {
                            one = array[i];
                        }
                    }
                   
                }
            }
            return one;
        }
        static void Main(string[] args)
        {
            int[] test = new int[] { 1,1,1,1,2,1,1,1,1};
            int result = Stray(test);
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}
