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
        public void Show()
        {           
          Console.WriteLine($"Room Height {Height},Room Width {Width}, Room Length {Length}");            
        }
    }
    static class Extention
    {
        public static void ShowRoomCeilingArea(this Room[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine("Ceiling area for room {0}", i);
                Console.WriteLine(array[i].CeilingArea());
            }
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
                Console.WriteLine($"Room {i} Height {RoomsArray[i].Height}");
                Console.WriteLine($"Room {i} Width {RoomsArray[i].Width}");
                Console.WriteLine($"Room {i} Length {RoomsArray[i].Length}");
            }         

            RoomsArray.ShowRoomCeilingArea();              
        }        

        public Room this[int x]
            {
                get
                {
                    try
                    {                      
                        return RoomsArray[x];
                    }
                    catch (NullReferenceException)
                    {
                        Console.WriteLine("NullReferenceException");
                        return new Room(0,0,0);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("IndexOutOfRangeException");
                        return new Room(0,0,0);
                    }           
              
                }
            }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Office office = new Office(2, 0.12);
            office.ShowRoomsInfo();
            office[2].Show();
            office[0].Show();
            office[1].Show();
            
            Console.ReadKey();
        }
    }
}
