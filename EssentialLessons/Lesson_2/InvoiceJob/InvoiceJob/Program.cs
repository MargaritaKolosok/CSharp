using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Создать класс Invoice. 
В теле класса создать три поля int account, string customer, string provider,
которые должны быть проинициализированы один раз (при создании экземпляра данного класса)
без возможности их дальнейшего изменения. В теле класса создать два закрытых поля string article,
int quantity.

 Создать метод расчета стоимости заказа с НДС и без НДС. Написать программу,
которая выводит на экран сумму оплаты заказанного товара с НДС или без НДС. 
 * */

class Invoice
{
    readonly int account;
    readonly string  customer;
    readonly string provider;      

    public Invoice(int account, string customer, string provider)
    {
        this.account = account;
        this.customer = customer;
        this.provider = provider;
    }

    public void CountPrice()
    {
        Console.WriteLine("Enter Price of the article");
        int quantity = Convert.ToInt32(Console.ReadLine());               

        NDS countNDS = new NDS(quantity , account);
        Console.WriteLine("Customer {0} has made order of article with price {1}$ / 1ent by Provider {2}. Price to Pay:", customer, account, provider);
        Console.WriteLine("Price with NDS = {0}", countNDS.CountWithNds());
        Console.WriteLine("Price with NDS = {0}", countNDS.CountWithoutNds());
    }    
}

class NDS
{
   private int quantity;
   private double price;

    public NDS(int quantity, double price)
    {
        this.quantity = quantity;
        this.price = price;
    }

    public double CountWithNds()
    {
        return price * quantity + (price / 100) * 20;
    }

    public double CountWithoutNds()
    {
        return price * quantity;
    }
}
namespace InvoiceJob
{
    class Program
    {
        static void Main(string[] args)
        {
            Invoice myInvoice = new Invoice(14, "Rita", "New Balance");
            myInvoice.CountPrice();
            Console.ReadKey(); 
        }
    }
}
