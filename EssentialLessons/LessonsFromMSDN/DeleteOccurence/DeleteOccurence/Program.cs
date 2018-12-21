using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public static int[] DeleteNth(int[] arr, int x)
{
    List<int> list = new List<int> { };

    for (int i=0; i<arr.Length; i++)
    {
        int count = 0;
        for (int j=0; j<arr.Length; j++)
        {
            if (count<x)
            {
                list.Add(arr[i]);
            }
        }
    }
}
namespace DeleteOccurence
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
