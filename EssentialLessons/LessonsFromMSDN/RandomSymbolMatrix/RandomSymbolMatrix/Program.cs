using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Matrix
{
    string[,] arr;
    string[] symb = {"*" , "+"};
    Random random = new Random();
    int X, Y;

    public Matrix(int x, int y)
    {
        X = x;
        Y = y;

        arr = new string[x,y] ;
        for (int i=0; i<x; i++)
        {
            for(int j=0; j<y; j++)
            {
                arr[i,j] = symb[random.Next(symb.Length)];
            }
        }

    }
    public void Show()
    {
        for (int i = 0; i <X; i++)
        {
            for (int j = 0; j <Y; j++)
            {
                Console.Write(arr[i, j]);               
            }
            Console.WriteLine();
        }
    }
}

namespace RandomSymbolMatrix
{
    class Program
    {
        static void Main(string[] args)
        {
            Matrix mat = new Matrix(3,4);
            mat.Show();
            Console.ReadKey();
        }
    }
}
