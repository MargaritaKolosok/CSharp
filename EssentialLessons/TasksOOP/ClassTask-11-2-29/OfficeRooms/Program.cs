using System;

namespace OfficeRooms
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
            return 2 * (height * length) + 2 * (width * height);
        }
    }
    class Office
    {
        int room;
        double paintAmount;

        Room[] RoomsArray;

        public Office(int room, double paintAmount)
        {
            this.room = room;
            this.paintAmount = paintAmount;

            CreateRooms();
        }

        private void CreateRooms()
        {
            for (int i=0; i<room; i++)
            {
                Room r;
                Console.WriteLine("Room Width");
                double w = Convert.ToDouble(Console.ReadLine());

                Console.WriteLine("Room height");
                double h = Convert.ToDouble(Console.ReadLine());

                Console.WriteLine("Room Length");
                double l = Convert.ToDouble(Console.ReadLine());

                r = new Room(w,h,l);

                RoomsArray[i] = r;
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
