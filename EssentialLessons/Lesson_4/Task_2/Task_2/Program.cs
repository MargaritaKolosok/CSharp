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
   public abstract void Open();
   public abstract void Create();
   public abstract void Change();
   public abstract void Save();
}
class XMLHandler : AbstractHandler
{
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
    public AbstractHandler ChooseFile(string file)
    {
        string formatFile = "";
        int s = file.IndexOf('.');
        formatFile = file.Substring(s);

        AbstractHandler doc;

        switch (formatFile)
        {
            

            case "txt":
                {
                    doc = new TXTHandler();
                    break;
                }
            case "xml":
                {
                    doc = new XMLHandler();
                    break;
                }
            case "doc":
                {
                    doc = new DOCHandler();
                    break;
                }
            default:
                {
                    doc = new DOCHandler();
                    break;
                }
                
        }
        return doc;

    }
}
namespace Task_2
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }
}
