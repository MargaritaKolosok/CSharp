using System;

namespace OfficeRooms
{
    class Room
    {
         double height;
         double width;
         double length;

        public double Height
        {
            get { return height; }
        }
        public double Width
        {
            get { return width; }
        }
        public double Length
        {
            get { return length; }
        }

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
            RoomsArray = new Room[room];
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

        public void ShowRoomsInfo()
        {
            for (int i =0; i < RoomsArray.Length; i++)
            {
                Console.WriteLine(RoomsArray[i].Height);
                Console.WriteLine(RoomsArray[i].Width);
                Console.WriteLine(RoomsArray[i].Length);
            }
        }

        public void ShowRoom()
        {
            
        }

        public Room this[int x]
            {
              get
                {
                    return RoomsArray[x];
                } 
            }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Office office = new Office(2, 0.12);
            office.ShowRoomsInfo();
            office[1]
            Console.ReadKey();
        }
    }
}
