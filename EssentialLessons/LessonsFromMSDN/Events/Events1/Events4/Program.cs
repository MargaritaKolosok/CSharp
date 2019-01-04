using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events4
{
    class AccountEventArgs : EventArgs
    {
        public string Message { get; } 
        public int Sum { get; }

        public AccountEventArgs(string message, int sum)
        {
            Message = message;
            Sum = sum;
        }
    }
    class Account
    {
        public delegate void AccountHandler(object sender, AccountEventArgs e);

        public event AccountHandler Added;
        public event AccountHandler Taken;

        int sum;

        public Account(int sum)
        {
            this.sum = sum;
        }

        public void Put(int sum)
        {
            this.sum += sum;
            Added?.Invoke(this, new AccountEventArgs($"You've received {sum} on conto", sum));
        }
        public void Take(int sum)
        {
            if (this.sum >= sum)
            {
                this.sum -= sum;
                Taken?.Invoke(this, new AccountEventArgs($"You've taken {sum} from conto", sum));
            }
            else
            {
                Taken?.Invoke(this, new AccountEventArgs($"You can't take {sum} from conto", sum));
            }
        }
       public int ShowSum
        {
            get { return sum; }
        }
    }

    static class Message
    {
        public static void ShowMessage(object sender, AccountEventArgs e)
        {
            Console.WriteLine($"Transaction {e.Sum}");
            Console.WriteLine(e.Message);

        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Account myAccount = new Account(2000);
            myAccount.Added += Message.ShowMessage;
            myAccount.Taken += Message.ShowMessage;
            myAccount.Put(200);
            myAccount.Take(600);
            Console.ReadKey();
        }
    }
}
