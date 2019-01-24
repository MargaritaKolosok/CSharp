using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsFiddle10
{
    public interface IDrawingObject
    {
        event EventHandler OnDrawn;
    }
    public interface IShape
    {
        event EventHandler OnDrawn;
    }

    public class Shape : IDrawingObject, IShape
    {

    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
