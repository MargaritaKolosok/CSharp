using System;

namespace Task_2_Part_2
{
    class Program
    {
        static void Main(string[] args)
        {
            int i, j, h = 6, w = 6;

            // Console.Write("*"), Console.Write(" "), Console.Write("\n") (для перехода на новую строку).

            for (i=0; i<w; i++)
            {

                for (j=0; j<h; j++)
                {

                    if (i == 0 || j ==0)
                    {

                        Console.Write("*");

                    }
                    else if (i + j == 6)
                    {
                        Console.Write("*");
                    }
                    else
                    {

                        Console.Write(" ");

                    }

                }

                Console.Write("\n");

            }

            Console.ReadKey();
        }
    }
}
