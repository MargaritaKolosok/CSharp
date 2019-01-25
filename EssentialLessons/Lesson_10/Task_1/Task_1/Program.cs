using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Используя Visual Studio, создайте проект по шаблону Console Application.
Создайте класс MyClass<T>, содержащий статический фабричный метод – T FacrotyMethod(),
который будет порождать экземпляры типа, указанного в качестве параметра типа (указателя места
заполнения типом – Т). 
 * */
namespace Task_1
{
    class MyClass<T> where T: new ()
    {        
        T variable;    

        public static T FactoryMethod()
        {
            return new T();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            int i = MyClass<int>.FactoryMethod();

            Console.WriteLine(i.GetType().Name);           

            Console.ReadKey();

        }
    }
}
