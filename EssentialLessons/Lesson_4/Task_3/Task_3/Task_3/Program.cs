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

}
namespace Task_3
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
