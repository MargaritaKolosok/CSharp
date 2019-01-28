using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4
{
    interface IMyList<T>
    {
        void Add(T item);
        int Count { get; }
    }
}
