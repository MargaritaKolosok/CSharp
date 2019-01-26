using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2
{
    class MyList<T> : IMyList<T>
    {
        T[] myList;
        int countElements = 0;

        public MyList()
        {
            myList = new T[0];
        }

        public void Add(T element)
        {            
            T[] temp = new T[myList.Length];            

            myList.CopyTo(temp,0);

            myList = new T[temp.Length+1];
            temp.CopyTo(myList, 0);
            myList[myList.Length - 1] = element;
                        
        }

       public T this[int index]
        {
            get
            {                
             return myList[index];              
            }
        }

        public int Count
        {
            get
            {
                return myList.Length;
            }
        }

        
    }
}
