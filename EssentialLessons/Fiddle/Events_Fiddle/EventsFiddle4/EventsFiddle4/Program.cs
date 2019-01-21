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
            MoneyAdded?.Invoke(this, new AccountEventArgs("$You Added {sum} on your account", sum));
        }
        public void Take(int sum)
        {
            if (this.sum >= sum)
            {
                this.sum -= sum;
                MoneyTaken?.Invoke(this, new AccountEventArgs("$You've taken {sum} from your account", sum));
            }
            else
            {
                MoneyTaken?.Invoke(this, new AccountEventArgs("$You've have not enough money on the conto. Current sum on conto is {sum}", sum));
            }
        }

    }

    class Program
    {
        static void Message(object sender, AccountEventArgs e)
        {
            Console.WriteLine("Sum of transaction {0}",e.Sum);
            Console.WriteLine(e.Message);
        }

        static void Main(string[] args)
        {
            Account myAccount = new Account(2000);
            myAccount.MoneyAdded += Message;
            myAccount.MoneyTaken += Message;


            myAccount.Put(200);
            myAccount.Take(1000);

            Console.ReadKey();
        }
    }
}
