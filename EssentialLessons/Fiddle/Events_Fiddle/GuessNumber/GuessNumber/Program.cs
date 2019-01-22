using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessNumber
{
    class GuessNumberArgs : EventArgs
    {
        public int Num { get; set; }
        public int Tries { get; set; }
        public string Result { get; set; }        
    }

    class RandomNum
    {
        int tries = 0;
        int randomNum;       

        public RandomNum()
        {
            randomNum = (new Random()).Next(10);
        }

        public void CheckNum(int userNum)
        {
            if (userNum == randomNum && tries < 5)
            {
                GuessNumberArgs args = new GuessNumberArgs
                {
                    Num = userNum,
                    Tries = tries,
                    Result = "You WIN!"
                };

                OnGameFinished(args);
            }            
        }

        event EventHandler<GuessNumberArgs> NumberGuessed;
        event EventHandler<GuessNumberArgs> NumberNotGuessed;

        protected virtual void OnGameFinished(GuessNumberArgs e)
        {
            Console.WriteLine($"");
        }

    }

    class Program
    {
       

        static void Main(string[] args)
        {
            RandomNum rNum = new RandomNum();

        }
    }
}
