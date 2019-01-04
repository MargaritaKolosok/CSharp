using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events3
{
    class Account
    {
        public delegate void AccountStateHandler(string message);

        public event AccountStateHandler MoneyTaken;
        public event AccountStateHandler MoneyAdded;

        int sum;

        public Account(int sum)
        {
            this.sum = sum;
        }

        public int CurrentSum
        {
            get { return sum; }
        }

        public void Put(int sum)
        {
            this.sum += sum;
            MoneyAdded?.Invoke($"Sum put on account {sum}");
        }

        public void Take(int sum)
        {
            if (this.sum > sum)
            {
                MoneyTaken?.Invoke($"Sum get from account {sum}");
            }
            else
            {
                MoneyTaken?.Invoke("Not enought money on the conto");
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
