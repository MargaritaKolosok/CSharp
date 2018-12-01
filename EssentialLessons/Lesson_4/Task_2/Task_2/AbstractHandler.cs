using System;
using System.Collections.Generic;
using System.Text;

namespace Task_2
{
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
            : base(fileName)
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
}
