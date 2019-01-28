using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
/*
В коллекцию ArrayList, через вызов метода Add добавьте элементы структурного и ссылочного типа,
переберите данную коллекцию с помощью, цикла for – С какой проблемой вы столкнулись? */
namespace Task_1
{
    class MyClass
    {

    }
    class Program
    {
        static void Main(string[] args)
        {
            System.Collections.ArrayList arrayList = new System.Collections.ArrayList();

            arrayList.Add(1);
            arrayList.Add("jahdajhsd");
            arrayList.Add(new MyClass());

            for (int i=0; i<arrayList.Count; i++)
            {
                Console.WriteLine(arrayList[i]);
            }

            Console.ReadKey();
            
        }
    }
}
