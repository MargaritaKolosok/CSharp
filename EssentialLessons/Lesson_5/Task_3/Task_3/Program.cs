using System;
/*
Создать класс MyMatrix, обеспечивающий представление матрицы произвольного размера
с возможностью изменения числа строк и столбцов.  
Написать программу, которая выводит на экран матрицу и производные от нее матрицы разных порядков.  
 * */
namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
        {
            MyMatrix matrix = new MyMatrix(5);          
            matrix.ShowMatrix();

            Console.WriteLine("-", 20);

            MyMatrix matrix2;
            matrix2 = MyMatrix.Copy(matrix, 8);
            matrix2.ShowMatrix();

            Console.ReadKey();
        }
    }
}
