using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
Создайте 2 интерфейса IPlayable и IRecodable. В каждом из интерфейсов создайте по 3 метода void
Play() / void Pause() / void Stop() и void Record() / void Pause() / void Stop() соответственно.
Создайте производный класс Player от базовых интерфейсов IPlayable и IRecodable.
Написать программу, которая выполняет проигрывание и запись. 
 
     */

interface IPlayable
{
    void Play();
    void Pause();
    void Stop();
}

interface IRecordable
{
    void Record();
    void Pause();
    void Stop();
}

class Player : IPlayable, IRecordable
{
    public void Play()
    {
        Console.WriteLine("Start Playing");
    }
    public void Record()
    {
        Console.WriteLine("Start Recording");
    }
    void IPlayable.Pause()
    {
        Console.WriteLine("Pause Playing");
    }
    void IPlayable.Stop()
    {
        Console.WriteLine("Stop Playing");
    }
    void IRecordable.Pause()
    {
        Console.WriteLine("Pause Recording");
    }
    void IRecordable.Stop()
    {
        Console.WriteLine("Stop Recording");
    }
}

namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
        {
            Player myPlayer = new Player();

            myPlayer.Play();            
            (myPlayer as IRecordable).Record();
            myPlayer.Record();
            (myPlayer as IPlayable).Stop();

            IRecordable myPlayer1 = new Player();
            myPlayer1.Pause();

            Console.ReadKey();
        }
    }
}
