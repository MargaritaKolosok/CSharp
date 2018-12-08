using System;
using System.Collections.Generic;
using System.Text;
/*
Создать класс Article, содержащий следующие закрытые поля:
• название товара;
• название магазина, в котором продается товар;
• стоимость товара в гривнах.
 * 
 * 
*/
 
namespace Task_4
{
    class Article
    {
        private string product;
        private string shop;
        private double price;

        public string Product
        {
            set { product = value; }
            get { return product; }
        }
        public string Shop
        {
            set { shop = value; }
            get { return shop; }
        }
        public double Price
        {
            set {

                if (value > 0)
                {
                    price = value;
                }
                else
                {
                    Console.WriteLine("Price can not be 0 or less then 0");
                }
                
                }
            get { return price; }
        }
        public Article(string product, string shop, double price)
        {
            Product = product;
            Shop = shop;
            Price = price;
        }

        public string Info()
        {
            return string.Format("Product: {0}, Shop: {1}, Price: {2}", Product, Shop, Price);
        }
        
    }
}
