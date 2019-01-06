using System;

namespace ClassTask_11_2_29
{
    class Room
    {
        protected double height;
        protected double width;
        protected double length;

        public Room(double height, double width, double length)
        {
            this.height = height;
            this.width = width;
            this.length = length;
        }
        public double CeilingArea()
        {
            return width * length;
        }
        public double WallsArea()
        {
            return 2* (height * length) + 2 * (width * height);
        }
    }

    class Office : Room
    {
        int rooms;
        double paint;
        int windowArea = 30;

        public Office(double height, double width, double length, int rooms, double paint) : base(height, width, length)
        {
            this.rooms = rooms;
            this.paint = paint;
        }

        public double CountAmountOfPaint()
        {
            return rooms * paint * ((CeilingArea() + WallsArea()) - windowArea);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Office office = new Office(12, 5, 10, 3, 0.12);
            Console.WriteLine($"CeilingArea {office.CeilingArea()}");
            Console.WriteLine($"Walls Area {office.WallsArea()}");
           
            Console.WriteLine($"Amount of Paint needed {office.CountAmountOfPaint()}");
            Console.ReadKey();
        }
    }
}
