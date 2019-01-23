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
            int tries = 1;

            Console.WriteLine("You have 3 tries to guess number from 1 to 10");

            for (int i=0; i<3; i++)
            {
                Console.WriteLine($"Try #{tries} Enter num:");
                userNum = int.Parse(Console.ReadLine());

                if (numToGuess == userNum)
                {
                    CustomEventArgs args = new CustomEventArgs();
                    OnNumberGuessed(args);
                }
                if (tries >= 3 && numToGuess != userNum)
                {
                    CustomEventArgs args = new CustomEventArgs();
                    OnLose(args);
                }

                tries++;
            }

            void OnNumberGuessed(CustomEventArgs e)
            {
                e.Message = "You WIN";
                e.Tries = tries;
                e.Num = userNum;

                GameFinished?.Invoke(this, e);                         
              
            }
            void OnLose(CustomEventArgs e)
            {
                e.Message = "You lose";
                e.Tries = tries;
                e.Num = numToGuess;
                GameFinished?.Invoke(this, e);
            }
        }
    }

    class Program
    {
        static void HandleCustomEvent(object sender, CustomEventArgs e)
        {
            Console.WriteLine("Game result:");
            Console.WriteLine(e.Message);
            Console.WriteLine($"Num in Mind {e.Num}");
            Console.WriteLine($"Tries: {e.Tries}");
        }
        static void Main(string[] args)
        {
            GuessNum guess = new GuessNum();
            guess.GameFinished += HandleCustomEvent;

            guess.Start();

            Console.ReadKey();
        }
    }
}
