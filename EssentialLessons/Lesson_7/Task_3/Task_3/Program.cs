using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Создайте класс MyClass и структуру MyStruct, которые содержат в себе поля public string change.
В классе Program создайте два метода:
- static void ClassTaker(MyClass myClass), который присваивает полю change экземпляра
myClass значение «изменено».
- static void StruktTaker(MyStruct myStruct), который присваивает полю change экземпляра
myStruct значение «изменено».
Продемонстрируйте разницу в использовании классов и структур, создав в методе Main() экземпляры
структуры и класса. Измените, значения полей экземпляров на «не изменено», а затем вызовите методы
ClassTaker и StruktTaker. Выведите на экран значения полей экземпляров. Проанализируйте
полученные результаты.  */

class MyClass
{
    public string change;
}

struct MyStruct
{
    public string change;
}
namespace Task_3
{
    class Program
    {
        static void ClassTaker(MyClass myClass)
        {
            myClass.change = "Changed";
        }

        static void StruktTaker(MyStruct myStruct)
        {
            myStruct.change = "Changed";
        }


        static void Main(string[] args)
        {
            MyClass classOb = new MyClass();
          //  Console.WriteLine(classOb.change);

            ClassTaker(classOb);
            Console.WriteLine(classOb.change);


            MyStruct structOb = new MyStruct();
            StruktTaker(structOb);
            Console.WriteLine(structOb.change);

            classOb.change = "Not changed";
            Console.WriteLine(classOb.change);

            structOb.change = "Not changed";
            Console.WriteLine(structOb.change);

            
            Console.ReadKey();
            
        }
    }
}
