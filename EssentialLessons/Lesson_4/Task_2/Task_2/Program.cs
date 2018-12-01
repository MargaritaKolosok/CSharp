using System;
using Task_2;

/*
Создайте класс AbstractHandler.
В теле класса создать методы void Open(), void Create(), void Chenge(), void Save().
Создать производные классы XMLHandler, TXTHandler, DOCHandler от базового класса
AbstractHandler.
Написать программу, которая будет выполнять определение документа и для каждого формата должны
быть методы открытия, создания, редактирования, сохранения определенного формата документа.
 */

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
