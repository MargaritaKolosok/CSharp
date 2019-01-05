using System;

namespace Property
{
    class PropertyAccess
    {
        int prop = 0;

        public int Property
            {
            get { return prop; }
            private set { prop = value; }
            }
        public void IncrementProperty()
        {
            Property++;
        }

    }
    class Program
    {
        static void Main(string[] args)
        {
            PropertyAccess prop = new PropertyAccess();
            Console.WriteLine(prop.Property);
            prop.IncrementProperty();
            prop.IncrementProperty();
            prop.IncrementProperty();

            Console.WriteLine(prop.Property);            
            Console.ReadKey();
        }
    }
}
