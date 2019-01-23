using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsFiddle9
{
    public class ShapeEventArgs : EventArgs
    {
        double newArea;

        public ShapeEventArgs(double newArea)
        {
            this.newArea = newArea;
        }
        public double NewArea
        {
            get { return newArea; }
        }
    }

    public abstract class Shape
    {
        protected double area;

        public double Area
        {
            get { return area; }
            set { area = value; }
        }

        public event EventHandler<ShapeEventArgs> ShapeChanged;

        public abstract void Draw();

        protected virtual void OnShapeChanged(ShapeEventArgs e)
        {
            EventHandler<ShapeEventArgs> handler = ShapeChanged;
            handler?.Invoke(this, e);
           
        }
    }

    public class Circle : Shape
    {
        double radius;

        double CircleArea(double radius)
        {
            return (3.14 * radius * radius);
        }

        public Circle(double radius)
        {
            this.radius = radius;
            area = CircleArea(radius);
        }

        public void Update(double radius)
        {
            this.radius = radius;
            area = CircleArea(radius);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
