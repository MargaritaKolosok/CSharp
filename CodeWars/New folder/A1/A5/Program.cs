﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A5
{
    public class Fighter
    {
        public string Name;
        public int Health, DamagePerAttack;
        public Fighter(string name, int health, int damagePerAttack)
        {
            this.Name = name;
            this.Health = health;
            this.DamagePerAttack = damagePerAttack;
        }
    }
    class Program
    {
        public static bool XO(string input)
        {
            return input.ToLower().Count(i => i == 'x') == input.ToLower().Count(i => i == 'o');
        }
        public static int Divisors(int n)
        {
            return Enumerable
                .Range(1, n)
                .Select(x => x)
                .Count(x => n % x == 0);
        }

        public static string PrinterError(String s)
        {
            s.ToLower();
            return s.Count(x => x > 'm') + "/" + s.Count();
        }

        public static int FindEvenIndex(int[] arr)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                if (arr.Take(i).Sum() == arr.Skip(i + 1).Sum()) { return i; }
            }
            return -1;
        }

        public static string OddOrEven(int[] array)
        {
            return (array.Sum() % 2 == 0) ? "even" : "odd";            
        }

        //    Write a function that takes a single
        //    string (word) as argument.
        //    The function must return an ordered list containing the indexes of all
        //    capital letters in the string.

        public static int[] Capitals(string word)
        {            
            return word
                .Select((c, i) =>
                Char.IsUpper(c) ? i : -1)
                .Where(i => i >= 0).ToArray();
        }
        public static string IsSortedAndHow(int[] array)
        {
            if (array.OrderBy(a => a).SequenceEqual(array)) return "yes accending";
            if (array.OrderByDescending(a => a).SequenceEqual(array)) return "Yes descending";
            else return "no";
        }

        public static string DuplicateEncode(string word)
        {
            string retval = "";
            word = word.ToLower();
            for (int i = 0; i < word.Length; i++)
                retval += (word.Split(word[i]).Length - 1 > 1 ? ')' : '(');
            return retval;
        }
        public static bool IsSquare(int n)
        {
            return Math.Sqrt(n) % 1 == 0;
        }
        public static bool comp(int[] a, int[] b)
        {
            int[] temp = a.Select(x => x * x).ToArray();
            Array.Sort(b);
            Array.Sort(temp);
            return temp.SequenceEqual(b);
        }
        public static int GetSum(int a, int b)
        {
            int A, B;
           
            if (a > b)
            {
                A = a;
                B = b;
            } else 
            {
                A = b;
                B = a;
            } 
            IEnumerable<int> query = Enumerable.Range(B, A).Select(x => x);
            //Good Luck!
            return (a != b) ? query.ToArray().Sum() : a;
        }

        public static long FindNextSquare(long num)
        {
            return ((Math.Sqrt(num) % 1 == 0)) ? (long)Math.Pow(Math.Sqrt(num) + 1, 2) : -1;            
        }

        public static List<int> RemoveSmallest(List<int> numbers)
        {
            numbers.Remove(numbers.DefaultIfEmpty().Min());
            return numbers;
        }
        public static int CountLettersAndDigits(string input)
        {
            //int result = 0;
            //int result = 0;
            //foreach (char x in input)
            //{
            //    if (Char.IsDigit(x) || Char.IsLetter(x))
            //    {
            //        result++;
            //    }
            //}

            return input.Count(c => Char.IsDigit(c) || Char.IsLetter(c));
        }
        public static string AlphabetPosition(string text)
        {
            //text.Select(x => x = text.Replace());
            ////Replace(text.Select(x => x), (char)alphabet.IndexOf((char)text.Select(x => x)));
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            
            foreach(char x in text)
            {
                text.Replace(x, Convert.ToChar(alphabet.IndexOf(x)));
            }
            return text;
        }
        public static string Rot13(string message)
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz";

            string result = "";

            foreach (char c in message)
            {
                if(Char.IsUpper(c))
                {
                    Char.ToLower(c);
                    if (alphabet.IndexOf(c) < alphabet.Length - 13)
                    {
                        result += Char.ToUpper(alphabet.ElementAt(alphabet.IndexOf(c) + 13));
                    }
                    else if(alphabet.IndexOf(c) >= alphabet.Length - 13)
                    {
                        int x = alphabet.Length - alphabet.IndexOf(c);
                        int left = 13 - x;
                        result += Char.ToUpper(alphabet.ElementAt(left));
                    }
                }
                else
                {
                    if (alphabet.IndexOf(c) < alphabet.Length - 13)
                    {
                        result += alphabet.ElementAt(alphabet.IndexOf(c) + 13);
                    }
                    else
                    {
                        int x = alphabet.Length - alphabet.IndexOf(c);
                        int left = 13 - x;
                        result += alphabet.ElementAt(left);
                    }
                }
                
            }
            return result;
        }
        public static string Order(string words)
        {
            
            string[] wordsArray = words.Split(new char[] { ' ' });
            string[] list = new string[wordsArray.Length];
            foreach(string s in wordsArray)
            {
                
                foreach(char c in s)
                {
                    if(Char.IsDigit(c))
                    {
                        
                        int x = (int)Char.GetNumericValue(c);
                        Console.WriteLine(x);                        
                        list[x-1] = s;
                    }
                }
            }

            return String.Join(" ", list);
        }
        static void Main(string[] args)
        {
            Console.WriteLine(XO("xxxooXXOo"));

            Console.WriteLine(Divisors(100));
             // 1,2,4,5,10,20,25,50,100

            Console.WriteLine(PrinterError("aaaxbbbbyyhwawiwjjjwwm"));

            int[] oddOrEvenArray = { 1, 2, 3, 4, 5, 6, 7, 8, 99, 3,4,4,5,5,5 };
            Console.WriteLine(OddOrEven(oddOrEvenArray));
            Console.WriteLine(DuplicateEncode("abca"));
            Console.WriteLine(GetSum(1,-1));

            Console.WriteLine(Rot13("test"));
            Console.WriteLine(Rot13("Test"));


            // "is2 Thi1s T4est 3a"
            Console.WriteLine(Order("is2 Thi1s T4est 3a"));
            Console.ReadLine();
        }
    }
}