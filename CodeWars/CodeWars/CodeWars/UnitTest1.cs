using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeWars
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Select_odd_number_in_Range()
        {
            var x = from i in Enumerable.Range(1, 10)
                    where (i % 2 == 0)
                    select i;

            foreach (var num in x)
            {
                Console.WriteLine(num + "\n");
            }
        }
        [Test]
        public string SumOfAllCollectionElements(int n)
        {
            if(n<0)
            {
                return string.Concat(n, "<", "0");
            }
            else
            {
                IEnumerable<int> result = Enumerable.Range(0, n + 1).Select(i => i);
                var sumstr = string.Join('+', result);
                return (n != 0) ? string.Concat(sumstr, " = ", result.Sum()) : string.Concat(sumstr, "=", result.Sum());
            }
        }

        [TestCase(2, 2, 2, ExpectedResult = 2)]
        [TestCase(2, 6, 2, ExpectedResult = 12)]
        [TestCase(1, 5, 1, ExpectedResult = 15)]
        [TestCase(1, 5, 3, ExpectedResult = 5)]
        public int SumTest(int begin, int end, int step)
        {
            var sum = 0;

            for (int i = begin; i <= end; i += step)
            {
                sum += i;
            }           

            return sum;

        }
        [TestCase("love", ExpectedResult =54)]
        [TestCase("friendship", ExpectedResult =108)]
        public int SumOfIndexs(string wordToCount)
        {
            string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var lettersSum = 0;

            foreach(var letter in wordToCount.ToUpper())
            {
              //  Console.WriteLine("{0}, {1}", letter, alphabet.IndexOf(letter) + 1);
                lettersSum += alphabet.IndexOf(letter)+1;
            }

            return lettersSum;
        }

        [TestCase("This is an example!", ExpectedResult = "sihT si na !elpmaxe")]
        [TestCase("double  spaces", ExpectedResult = "elbuod  secaps")]
        public string ReturnRevertedStrins(string str)
        {
            return string.Join(" ", str.Split(" ").Select(word => string.Concat(word.Reverse())));

        }

        [TestCase(("1 2 3 4 5"))]
        public string SelectMinAndMax(string numbers)
        {            
            var strArray = numbers.Split(' ');
            List<int> listOfNumbers = new List<int>();
            
            foreach(var c in strArray)
            {
                int.TryParse(c, out int number);
                listOfNumbers.Add(number);
            }
            var max = listOfNumbers.Max();
            var min = listOfNumbers.Min();

            return string.Join(" ", listOfNumbers.Max(), listOfNumbers.Min());
        }
        [TestCase(("1 2 3 4 5"))]
        public string SelectMinAndMax2(string numbers)
        {

            var result = numbers.Split(' ').Select(x => Convert.ToInt32(x));

            return string.Join(" ", result.Max(), result.Min());
        }
    }
}