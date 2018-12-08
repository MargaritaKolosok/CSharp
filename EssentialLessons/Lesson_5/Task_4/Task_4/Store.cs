using System;
using System.Collections.Generic;
using System.Text;
/*Создать класс Store, содержащий закрытый массив элементов типа Article.
Обеспечить следующие возможности:
• вывод информации о товаре по номеру с помощью индекса;
• вывод на экран информации о товаре, название которого введено с клавиатуры,
если таких товаров нет, выдать соответствующее сообщение;*/
namespace Task_4
{
    class Store
    {
        Article[] storeArr;

        public Store(int n)
        {
            storeArr = new Article[n];

            for (int i=0; i<n; i++)
            {
                Console.WriteLine("Введите имя товара {0}", i);
                string name = Convert.ToString(Console.ReadLine());
                storeArr[i].Product = name;

                Console.WriteLine("Введите магазин {0}", i);
                string shop  = Convert.ToString(Console.ReadLine());
                storeArr[i].Shop = shop;

                Console.WriteLine("Введите цену товара {0}", i);
                double price = Convert.ToDouble(Console.ReadLine());
                storeArr[i].Price = price;
            }       
        }

        public string this[string product]
        {
            get
            { 
                for (int i = 0; i < storeArr.Length; i++)
                {
                    if (storeArr[i].Product == product)
                        return storeArr[i].Info();
                }

                return string.Format("\"{0}\" нет в наличии.", product);
            }
        }

        public string this[int index]
        {
            get
            {
                if (index < storeArr.Length && index >=0)
                {
                    return storeArr[index].Info();
                }
                return "Попытка обращения за пределы массива.";
            }
        }
    }
}
