using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeWars
{

    public class Day2
    {

        [Datapoint]
        string[] s1 = new String[] { "vJQ", "anj", "mQDq", "sOZ" };
       
        [Theory]
        public void DivideArrayInPairs(string[] arr)
        {
            var result =  Enumerable.Range(1, arr.Length - 1)
                           .Select(x =>
                               new string[]{
                                 string.Join(" ",arr.Take(x)),
                                 string.Join(" ",arr.Skip(x).Take(arr.Length-x))
                                 })
                           .ToArray();
            foreach(var array in result)
            {
                foreach(var el in array)
                {
                    Console.WriteLine(el.ToString());
                }
                
            }

            

        }

        [Test]
        public string[] TowerBuilder(int nFloors)
        {
            var result = new string[nFloors];
            for (int i = 0; i < nFloors; i++)
                result[i] = string.Concat(new string(' ', nFloors - i - 1),
                                          new string('*', i * 2 + 1),
                                          new string(' ', nFloors - i - 1));
            return result;
        }
    }
}
