using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsFiddle_8
{
    class CustomEventArgs : EventArgs
    {
        public string Message { get; set; }
        public int Tries { get; set; }
        public int Num { get; set; }
    }

    class GuessNum
    {
        public event EventHandler<CustomEventArgs> GameFinished;

        public void Start()
        {           
            int numToGuess = (new Random()).Next(1,10);
            int userNum;

            Console.WriteLine("You have 3 tries to guess number from 1 to 10");

            for (int i=0; i<3; i++)
            {
                Console.WriteLine($"Try #{i} Enter num:");
                userNum = int.Parse(Console.ReadLine());

                if (numToGuess == userNum)
                {

                }
            }

            void OnNumberGuessed(CustomEventArgs e)
            {

            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
