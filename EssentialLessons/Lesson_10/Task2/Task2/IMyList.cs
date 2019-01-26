using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task2
{
    interface IMyList<T>
    {
       void Add(T element);
       T this[int index] { get; }
      //  int Count { get; }
      //  void Clear();
      //  bool Contains(T element);
    }
}
