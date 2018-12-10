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
    public void Count()
    {
        int star = 0, plus = 0;
        foreach (string str in arr)
        {
            if (str == "*")
            {
                star++;
            }
            else
            {
                plus++;
            }            
        }

        Console.WriteLine("Starts: {0}, plus {1}", star, plus);
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
            mat.Count();
            Console.ReadKey();
        }
    }
}
