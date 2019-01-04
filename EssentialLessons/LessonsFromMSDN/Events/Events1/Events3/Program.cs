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
            if (this.sum >= sum)
            {
                this.sum -= sum;
                MoneyTaken?.Invoke($"Sum get from account {sum}");
            }
            else
            {
                MoneyTaken?.Invoke("Not enought money on the conto");
            }
        }

    }
    class Message
    {
        public static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Account myAccount = new Account(2000);
            myAccount.MoneyAdded += Message.ShowMessage;
            myAccount.MoneyTaken += Message.ShowMessage;


            Console.WriteLine(myAccount.CurrentSum);

            myAccount.Put(200);
            Console.WriteLine(myAccount.CurrentSum);
            myAccount.Take(1200);
            Console.WriteLine(myAccount.CurrentSum);
            Console.ReadKey();


        }
    }
}
