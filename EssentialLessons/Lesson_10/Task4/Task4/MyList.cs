using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4
{
    class MyList<T> : IMyList<T>
    {
        T[] myList;

        public MyList()
        {
            myList = new T[0];
        }

        public void Add(T element)
        {
            T[] tempArray = new T[myList.Length];
            myList.CopyTo(tempArray, 0);
            myList = new T[myList.Length + 1];
            tempArray.CopyTo(myList,0);
            myList[myList.Length - 1] = element;
        }

        public int Count
        {
            get
            {
                return myList.Length;
            }
        }

        public T this[int i]
        {
            get
            {
                return myList[i];
            }
        }        
    }
}
