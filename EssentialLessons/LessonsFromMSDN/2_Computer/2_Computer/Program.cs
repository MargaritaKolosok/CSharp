using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
построить класс 1-го уровня с указанными в индивидуальном
задании (табл. 11.7) полями и методами:
- конструктор;

- функция, которая определяет «качество» объекта – Q по заданной фор-
муле (табл11. 7, столб 2);

- вывод информации об объекте.

Построить класс 2-го уровня (класс-потомок), который содержит:
- дополнительное поле P;
- функция, которая определяет «качество» объекта класса 2-го уровня –
Qp, которая перекрывает функцию качества класса 1-го уровня (Q ), выполняя
вычисление по новой формуле (табл. 11.7, столб 3).
Создать проект для демонстрации работы: ввод и вывод информации об
объектах классов 1-го и 2-го уровней.

 Компьютер:
- наименование процессора;
-тактовая частота процессора (МГц);
- объем оперативной памяти (Мб).
Q = (0,1·частота) + память

 P: объем винчестера (Гб)
Qp=Q+0,5·Р

 */

class ComputerBase
{
    private double frequency;
    private int memory;
    private string processorName;
  
    public ComputerBase(string p, double f, int mem)
        {
        processorName = p;
        frequency = f;
        memory = mem;
        }
    public virtual double Quality()
    {
        return ((0.1 * frequency) + memory);
    }
}
class Computer : ComputerBase
{
    private int hardDisk;
    public Computer(string p, double f, int mem, int h) : base(p,f,mem)
    {
        hardDisk = h;
    }
    public override double Quality()
    {
        return (base.Quality() + 0.5 * hardDisk);
    }
}

namespace _2_Computer
{
    class Program
    {
        static void Main(string[] args)
        {
            ComputerBase Comp1 = new ComputerBase("Intel 5", 2.2, 4);
            Computer Comp2 = new Computer("Intel 7", 2.2, 4, 500);
            Console.WriteLine("Comp1 Quality: {0}", Comp1.Quality());
            Console.WriteLine("Comp2 Quality: {0}", Comp2.Quality());
            Console.ReadKey();
        }
    }
}
