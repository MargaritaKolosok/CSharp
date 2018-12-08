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

            for (int i = 0; i < x; i++)
            {
                int k = random.Next(2, 7);
                matrix[i] = new int[k];
            }

            Fill();
        }
        private void Fill()
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    matrix[i][j] = random.Next(1, 100);
                }
            }
        }
        public void ShowMatrix()
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                for (int j = 0; j < matrix[i].Length; j++)
                {
                    Console.Write(" " + matrix[i][j]);
                }
                Console.WriteLine();
            }
        }

      protected int this[int i, int j]
        {
            get { return matrix[i][j]; }           
        }

       public static MyMatrix Copy(MyMatrix m, int x)
        {
            MyMatrix mat = new MyMatrix(x);

            int rowB;

            rowB = (mat.matrix.Length < m.matrix.Length)? mat.matrix.Length : m.matrix.Length;
            

            for (int i=0; i<rowB; i++)
            {
                int columnB = (mat.matrix[i].Length < m.matrix[i].Length ) ? mat.matrix[i].Length : m.matrix[i].Length;

                for (int j=0; j<columnB; j++)
                {                    
                    mat.matrix[i][j] = m.matrix[i][j];
                }
            }

            return mat;
        }

    }
}
