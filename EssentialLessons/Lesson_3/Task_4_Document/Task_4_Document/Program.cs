using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Создайте класс DocumentWorker. В теле класса создайте три метода OpenDocument(), EditDocument(), SaveDocument().   
В тело каждого из методов добавьте вывод на экран соответствующих строк:
"Документ открыт", "Редактирование документа доступно в версии Про",
"Сохранение документа доступно в версии Про".  

    Создайте производный класс ProDocumentWorker.
Переопределите соответствующие методы, при переопределении методов выводите следующие строки:
"Документ отредактирован", "Документ сохранен в старом формате,
сохранение в остальных форматах доступно в версии Эксперт".

Создайте производный  класс ExpertDocumentWorker от базового класса ProDocumentWorker.
Переопределите соответствующий метод. При вызове данного метода необходимо выводить на экран 
"Документ сохранен в новом формате". В теле метода Main() реализуйте возможность приема от
пользователя номера ключа доступа pro и exp. Если пользователь не вводит ключ, он может пользоваться
только бесплатной версией (создается экземпляр базового класса), если пользователь ввел номера ключа 
доступа pro и exp, то должен создаться экземпляр соответствующей версии класса, приведенный к базовому – DocumentWorker.  

 * */

class DocumentWorker
{
    public virtual void OpenDocument()
    {
        Console.WriteLine("Документ открыт");
    }

    public virtual void EditDocument()
    {
        Console.WriteLine("Редактирование документа доступно в версии Про");
    }
    public virtual void SaveDocument()
    {
        Console.WriteLine("Сохранение документа доступно в версии Про");
    }
}

class ProDocumentWorker : DocumentWorker
{

    public override void OpenDocument()
    {
        Console.WriteLine("Документ отредактирован");
    }
    public override void EditDocument()
    {
        Console.WriteLine("Документ сохранен в старом формате");
    }
    public override void SaveDocument()
    {
        Console.WriteLine("Cохранение в остальных форматах доступно в версии Эксперт");
    }
}

class ExpertDocumentWorker : ProDocumentWorker
{
    public override void SaveDocument()
    {
        Console.WriteLine("Документ сохранен в новом формате");
    }
}

    
namespace Task_4_Document
{
    class Program
    {
      static DocumentWorker WorkWithDocument(string version)
        {
             DocumentWorker doc;

            if (version == "pro")
            {
                 doc = new ProDocumentWorker();
            }
            else if (version == "exp")
            {
                 doc = new ExpertDocumentWorker();
            }
            else
            {
                 doc = new DocumentWorker();
            }

            return doc;
        }
        static void Main(string[] args)
        {
            string version;
            DocumentWorker myDoc;
            Console.WriteLine("Enter version of yout DocWorker:");
            version = Convert.ToString(Console.ReadLine());
            Console.OutputEncoding = Encoding.UTF8;
            myDoc = WorkWithDocument(version);
            myDoc.OpenDocument();
            myDoc.EditDocument();
            myDoc.SaveDocument();
            Console.ReadKey();

            
        }
    }
}
