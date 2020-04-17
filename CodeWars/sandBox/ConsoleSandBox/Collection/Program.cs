using System;
using System.Collections;

namespace Collection
{
    class Collection : IEnumerable
    {
        int[] array = new int[5] { 1, 2, 3, 45, 5 };
        public IEnumerator GetEnumerator()
        {
            return new Iterator(this);
        }

        class Iterator : IEnumerator
        {

            int[] array;
            int currentPosition = -1;

            public Iterator(Collection col)
            {
                array = col.array;
            }            

            public object Current
            {
                get
                {
                    return array[currentPosition];
                }                 
            }
            public bool MoveNext()
            {
                if(currentPosition < array.Length - 1)
                {
                    currentPosition++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                currentPosition = -1;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Collection col = new Collection();
            foreach(var item in col)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine(col.GetEnumerator());           
            

            Console.WriteLine("Hello World!");
        }
    }
}
