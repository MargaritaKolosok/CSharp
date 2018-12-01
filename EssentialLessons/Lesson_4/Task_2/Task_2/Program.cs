using System;
/*
Создайте класс AbstractHandler.
В теле класса создать методы void Open(), void Create(), void Chenge(), void Save().
Создать производные классы XMLHandler, TXTHandler, DOCHandler от базового класса
AbstractHandler.
Написать программу, которая будет выполнять определение документа и для каждого формата должны
быть методы открытия, создания, редактирования, сохранения определенного формата документа.
 */

abstract class AbstractHandler
{
    protected string fileName;

    public AbstractHandler(string fileName)
    {
        this.fileName = fileName;
    }
 
   public abstract void Open();
   public abstract void Create();
   public abstract void Change();
   public abstract void Save();
}
class XMLHandler : AbstractHandler
{
    public XMLHandler(string fileName)
        :base(fileName)
    {

    }
    public override void Open()
    {
        Console.WriteLine("XML Opened");
    }
    public override void Create()
    {
        Console.WriteLine("XML Created");
    }
    public override void Change()
    {
        Console.WriteLine("XML Changed");
    }
    public override void Save()
    {
        Console.WriteLine("XML Saved");
    }
}

class TXTHandler : AbstractHandler
{
    public TXTHandler(string fileName)
        : base(fileName)
    {

    }
    public override void Open()
    {
        Console.WriteLine("TXT Opened");
    }
    public override void Create()
    {
        Console.WriteLine("TXT Created");
    }
    public override void Change()
    {
        Console.WriteLine("TXT Changed");
    }
    public override void Save()
    {
        Console.WriteLine("TXT Saved");
    }
}

class DOCHandler : AbstractHandler
{
    public DOCHandler(string fileName)
        : base(fileName)
    {
    }
    public override void Open()
    {
        Console.WriteLine("DOC Opened");
    }
    public override void Create()
    {
        Console.WriteLine("DOC Created");
    }
    public override void Change()
    {
        Console.WriteLine("XDOC Changed");
    }
    public override void Save()
    {
        Console.WriteLine("DOC Saved");
    }
}
class Redactor 
{
    AbstractHandler doc;

    public void ChooseFile(string file)
    {
        string formatFile = "";
        int index = file.IndexOf('.');
        formatFile = file.Substring(index);
        string fileName = file; 

        

        switch (formatFile)
        {
            case ".txt":
                {
                    doc = new TXTHandler(fileName);
                    break;
                }
            case ".xml":
                {
                    doc = new XMLHandler(fileName);
                    break;
                }
            case ".doc":
                {
                    doc = new DOCHandler(fileName);
                    break;
                }
            default:
                {
                    Console.WriteLine("Unknown format");
                    break;
                }
        }
        
    }
    public void Open()
    {
        doc.Open();
    }
    public void Change()
    {
        doc.Change();
    }
    public void Save()
    {
        doc.Save();
    }
    public void Create()
    {
        doc.Create();
    }
}
namespace Task_2
{
    class Program
    {
        static void Main(string[] args)
        {
            Redactor redactor = new Redactor();
            redactor.ChooseFile("file.txt");
            redactor.Open();
            redactor.Change();
            redactor.Save();
            redactor.Create();
            Console.ReadKey();


        }
    }
}
