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
        }

        public void AddArticle(Article value, int index)
        {
            if (index >= 0 && index < storeArr.Length)
            {
                storeArr[index] = value;
            }
            else { Console.WriteLine("Попытка записи за пределами массива.");  }
                
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
