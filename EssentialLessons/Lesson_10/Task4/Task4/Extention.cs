using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4
{
    static class Extention
    {
        public static T[] GetArray<T>(this MyList<T> list)
        {
            T[] temp = new T[list.Count];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = list[i];
            }
            return temp;
        }
    }
}
