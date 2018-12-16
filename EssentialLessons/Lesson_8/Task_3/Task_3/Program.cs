using System;
/*
Создайте перечисление, в котором будут содержаться должности сотрудников как имена констант.
Присвойте каждой константе значение, задающее количество часов, которые должен отработать
сотрудник за месяц.
Создайте класс Accauntant с методом bool AskForBonus(Post worker, int hours), отражающее
давать или нет сотруднику премию. Если сотрудник отработал больше положеных часов в месяц, то ему
положена премия. 
 * */
enum Jobs { QA = 40, Progremmer = 30, Manager = 50, Support=45 };
class Accauntant
{
   public bool AskForBonus(Jobs worker, int hours)
    {
        bool result;
        if (worker == 0)
        {
            Console.WriteLine("Not such worker!");
            result = false;
        }
        else
        {
            if ((int)worker < hours)
            {
                Console.WriteLine("Worker {0} should get extra money", worker);
                result = true;
            }
            else
            {
                Console.WriteLine("Worker {0} should not get extra money", worker);
                result = false;
            }
        }
        return result;
    }
}
namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
        {
            Accauntant my = new Accauntant();
            
            Console.WriteLine(my.AskForBonus(Jobs.Progremmer, 56));
            Console.WriteLine(my.AskForBonus(Jobs.Progremmer, 23));
            Console.WriteLine(my.AskForBonus(Jobs.QA, 23));
            Console.ReadKey();
        }
    }
}
