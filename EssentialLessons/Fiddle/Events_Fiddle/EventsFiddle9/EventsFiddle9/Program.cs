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

        double CircleArea(double radius) => 3.14 * radius * radius;

        public Circle(double radius)
        {
            this.radius = radius;
            area = CircleArea(radius);
        }

        public void Update(double radius)
        {
            this.radius = radius;
            area = CircleArea(radius);
            ShapeEventArgs args = new ShapeEventArgs(area);
            OnShapeChanged(args);
        }
        protected override void OnShapeChanged(ShapeEventArgs e)
        {
            base.OnShapeChanged(e);
        }
        public override void Draw()
        {
            Console.WriteLine("Circle drawn");
        }
    }
    public class ShapeContainer
    {
        List<Shape> shapeList;

        public ShapeContainer()
        {
            shapeList = new List<Shape>();
        }

        public void AddShape(Shape shape)
        {
            shapeList.Add(shape);
            shape.ShapeChanged += Shape_ShapeChanged;
        }

        private void Shape_ShapeChanged(object sender, ShapeEventArgs e)
        {
            Shape shape = (Shape)sender;
            Console.WriteLine($"Event fires. Shape area is {e.NewArea}");
            shape.Draw();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Circle circle1 = new Circle(34);
            Circle circle2 = new Circle(3);

            ShapeContainer shapeContainr = new ShapeContainer();

            shapeContainr.AddShape(circle1);
            shapeContainr.AddShape(circle2);

            circle1.Update(23);
            circle2.Update(2);

            Console.ReadKey();
        }
    }
}
