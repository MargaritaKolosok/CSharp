using System;
using System.Collections.Generic;
using System.Text;

namespace Task_2
{
    class Redactor
    {
        AbstractHandler doc;

        public void ChooseFile(string file)
        {           
            int index = file.IndexOf('.');
            string  formatFile = file.Substring(index);
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
}
