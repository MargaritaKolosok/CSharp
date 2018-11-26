using System;
/*
 Создать класс, представляющий учебный класс ClassRoom.
 Создайте класс ученик Pupil.  В теле класса создайте методы void Study(),
 void Read(), void Write(), void Relax(). 
 Создайте 3 производных класса ExcelentPupil,
 GoodPupil, BadPupil  от класса базового класса Pupil и переопределите каждый из методов,
 в зависимости от успеваемости ученика.   Конструктор класса ClassRoom принимает аргументы типа Pupil, 
 класс должен состоять из 4 учеников. Предусмотрите возможность того,
 что пользователь может передать 2 или 3 аргумента. 
 Выведите информацию о том, как все ученики экземпляра класса ClassRoom умеют учиться, читать, писать, отдыхать.  
 
     * 
     */

class ClassRoom
{   
    public ClassRoom(int num)
    {
        string name;
        int note;

        Pupil[] pupils = new Pupil[num];
        
        for (int i = 0; i<num; i++)
        {
            Console.WriteLine("Enter Name of Pupil {0}", i +1);
            name = Convert.ToString(Console.ReadLine());
            Console.WriteLine("Enter general Pupils note: 3, 4 or 5");
            note = Convert.ToInt32(Console.ReadLine());

            switch (note)
            {
                case 3:
                    pupils[i] = new BadPupil();
                    pupils[i].Name = name;
                    break;
                case 4:
                    pupils[i] = new GoodPupil();
                    pupils[i].Name = name;
                    break;
                case 5:
                    pupils[i] = new ExcelentPupil();
                    pupils[i].Name = name;
                    break;
                default:
                    pupils[i] = new GoodPupil();
                    pupils[i].Name = name;
                    break;                    
            }
        }

        foreach (Pupil pupil in pupils)
        {
            Console.WriteLine(pupil.Name);
            pupil.Read();
            pupil.Write();
            pupil.Study();
        }
       
    }
  
      
}

public class Pupil
{
    string name;
    public string Name

    {
        set => name = value;
        get => name;
    }
    

    public virtual void Study()
    {
       
    }

    public virtual void Read()
    {
       
    }
    public virtual void Write()
    {
        
    }
    public virtual void Relax()
    {
        
    }
}
class ExcelentPupil : Pupil
{
    public override void Study()
    {   
        Console.WriteLine("Can Study very well");
    }

    public override void Read()
    {
        Console.WriteLine("Can Read very well");
    }
    public override void Write()
    {
        Console.WriteLine("Can Write very well");
    }
    public override void Relax()
    {
        Console.WriteLine("Can Relax very well");
    }
}
class GoodPupil : Pupil
{
    public override void Study()
    {
        Console.WriteLine("Can Study very good");
    }

    public override void Read()
    {
        Console.WriteLine("Can Read very good");
    }
    public override void Write()
    {
        Console.WriteLine("Can Write very good");
    }
    public override void Relax()
    {
        Console.WriteLine("Can Relax very good");
    }
}

class BadPupil : Pupil
{
    public override void Study()
    {
        Console.WriteLine("Can Study very bad");
    }

    public override void Read()
    {
        Console.WriteLine("Can Read very bad");
    }
    public override void Write()
    {
        Console.WriteLine("Can Write very bad");
    }
    public override void Relax()
    {
        Console.WriteLine("Can Relax very bad");
    }
}
namespace Task2_ClassTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            ClassRoom myClass = new ClassRoom(3);       
            Console.ReadKey();
        }
    }
}
