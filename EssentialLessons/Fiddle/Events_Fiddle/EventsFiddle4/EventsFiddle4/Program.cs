using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsFiddle4
{
    class AccountEventArgs
    {
        public string Message { get; }
        public int Sum { get; }

        public AccountEventArgs(string msg, int sum)
        {
            Message = msg;
            Sum = sum;
        }
    }

    class Account
    {
        public delegate void AccountStateHandler(object sender, AccountEventArgs e);

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
            MoneyAdded?.Invoke(this, new AccountEventArgs("$you Added {sum} on your account", sum));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
