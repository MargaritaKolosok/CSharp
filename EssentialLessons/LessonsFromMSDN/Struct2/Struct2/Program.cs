using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Создайте структуру с именем - Notebook.
Поля структуры: модель, производитель, цена.
В структуре должен быть реализован конструктор для инициализации полей и метод для вывода
содержимого полей на экран. 
*/
struct Notebook
{
    string model;
    string producer;
    double price;

    public Notebook(string model, string producer, double price)
    {
        this.model = model;
        this.producer = producer;
        this.price = price;
    }
    public void ShowInfo()
    {
        Console.WriteLine("Model {0}, producer {1}, Price {2} $", model, producer, price);
    }
}
namespace Struct2
{
    class Program
    {
        static void Main(string[] args)
        {
            Notebook myNote = new Notebook("A1543", "Asus", 1264);
            myNote.ShowInfo();
            Console.ReadKey();
        }
    }
}
