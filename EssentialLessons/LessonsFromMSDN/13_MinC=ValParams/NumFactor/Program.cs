using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
class Factor
{
    public List<int> Factors(int num, out int factors)
    {
        List<int> factorArray = new List<int>();
        int j = 0;
        for (int i=2; i<num/2+1; i++)
        {
            if (num%i==0)
            {
                factorArray.Add(i);
                j++;
            }
        }
        factors = j;
        return factorArray;
    }
}
namespace NumFactor
{
    class Program
    {
        static void Main(string[] args)
        {
            Factor myFactor = new Factor();
            List<int> myList = myFactor.Factors(1500, out int factors);
            foreach (int x in myList)
            {
                Console.WriteLine(x);
            }
           
            Console.WriteLine("Factors = {0}", factors);
            Console.ReadKey();

        }
    }
}
