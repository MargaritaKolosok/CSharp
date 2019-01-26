using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3
{
    class MyDictionary<TKey, TValue> 
    {
        Dictionary<TKey, TValue> myDictionary = new Dictionary<TKey, TValue>();

        public void Add(TKey key, TValue value)
        {
            myDictionary.Add(key, value);
        }

        public TValue this[TKey key]
        {
            get
            {
                if (myDictionary.TryGetValue(key, out TValue value))
                {
                    return value;
                }
                else
                {
                    Console.WriteLine("No such key exists");
                    return value;
                }
                
            }
        }

        public int Count
        {
            get
            {                
                return myDictionary.Count;
            }            
        }          
    }
}
