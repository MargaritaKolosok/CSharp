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
        event EventHandler PreDrawEvent;
        event EventHandler PostDrawnEvent;

        object objectLock = new Object();

        event EventHandler IDrawingObject.OnDrawn
        {
            add
            {
                lock (objectLock)
                {
                    PreDrawEvent += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    PreDrawEvent -= value;
                }
            }
        }

        event EventHandler IShape.OnDrawn
        {
            add
            {
                lock (objectLock)
                {
                    PostDrawnEvent += value;
                }
            }
            remove
            {
                lock (objectLock)
                {
                    PostDrawnEvent -= value;
                }
            }
        }

        public void Draw()
        {
            PreDrawEvent?.Invoke(this, EventArgs.Empty);
            Console.WriteLine("Drawing a shape");

            PostDrawnEvent?.Invoke(this, EventArgs.Empty);
        }
    }
    public class Subscriber1
    {
        public Subscriber1(Shape shape)
        {
            IDrawingObject s = (IDrawingObject)shape;
            s.OnDrawn += S_OnDrawn;
        }

        private void S_OnDrawn(object sender, EventArgs e)
        {
            Console.WriteLine("Sub1 received the IDrawingObject event");
        }
    }
    public class Subscriber2
    {
        public Subscriber2(Shape shape)
        {
            IShape s = (IShape)shape;
            s.OnDrawn += S_OnDrawn;
        }

        private void S_OnDrawn(object sender, EventArgs e)
        {
            Console.WriteLine("Subscriber 2 received IShape event");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Shape shape = new Shape();
            Subscriber1 sub1 = new Subscriber1(shape);
            Subscriber2 sub2 = new Subscriber2(shape);

            shape.Draw();
            Console.WriteLine();
            shape.Draw();

            Console.ReadKey();
        }
    }
}
