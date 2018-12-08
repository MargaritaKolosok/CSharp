using System;
using System.Collections.Generic;
using System.Text;

namespace Task_3
{
    class MyMatrix
    {
        int[][] matrix;
        Random random = new Random();

        public MyMatrix(int x)
        {
            matrix = new int[x][];

            for (int i=0; i<x; i++)
            {
                int k = random.Next(2,5);
                matrix[i] = new int[k];
                                
            }
            Fill();
        }
        private void Fill()
        {
            for (int i=0; i<matrix.Length; i++)
            {
                for (int j=0; j<matrix[i].Length; j++)
                {
                    matrix[i][j] = random.Next(1,100);
                }
            }
        }
        public void ShowMatrix()
        {
            for (int i=0; i<matrix.Length; i++)
            {
                for (int j=0; j<matrix[i].Length; j++)
                {
                    Console.Write(" " + matrix[i][j]);
                }
                Console.WriteLine();
            }
        }

    }
}
