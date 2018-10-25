using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
             * Используя Visual Studio, создайте проект по шаблону Console Application.  
                Представьте, что вы реализуете программу для банка,
                которая помогает определить, погасил ли клиент кредит или нет.
                Допустим, ежемесячная сумма платежа должна составлять 100 грн.
                Клиент должен выполнить 7 платежей, но может платить реже большими суммами.
                Т.е., может двумя платежами по 300 и 400 грн. закрыть весь долг.   
            Создайте метод, который будет в качестве аргумента принимать сумму платежа,
            введенную экономистом банка. Метод выводит на экран информацию о состоянии кредита (сумма задолженности, сумма переплаты, сообщение об отсутствии долга).  

             */

namespace Task_1
{
    class Program
    {
        static void CheckCredit(int debtSum, int counterPay)
        {
            int Amount()
            {
                Console.WriteLine("User bays to bank:");
                int amount = Convert.ToInt32(Console.ReadLine());
                return amount;
            }
      
            int transaction = Amount();

            if (debtSum <= transaction)
            {
                debtSum = transaction - debtSum;
                Console.WriteLine("You have overpaid: {0:C}, THERE IS NO DEBT!", debtSum, counterPay);
            }
            else if (debtSum != 0 && counterPay != 0)
            {
                debtSum -= transaction;
                counterPay--;
                Console.WriteLine("Debt Summ is: {0:C}, month to pay left: {1}", debtSum, counterPay);
                CheckCredit(debtSum, counterPay);
            }  
            else
            {
                Console.WriteLine("Debt Summ is: {0:C}, overdue loan {1}", debtSum, counterPay);
            }          
        }

        static void Main(string[] args)
        {
            int debt = 700;
            int counter = 7;
            
            CheckCredit(debt, counter);
            Console.ReadKey();

        }
    }
}
