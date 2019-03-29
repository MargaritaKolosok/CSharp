using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace LinqConsoleApp
{
    [Table(Name = "Customers")]
    public class Customer
    {
        private string _CustomerID;

        [Column(IsPrimaryKey = true, Storage = "_CustomerID")]

        public string CustomerID
        {
            get
            {
                return this._CustomerID;
            }
            set
            {
                this._CustomerID = value;
            }
        }

        private string _City;
        [Column(Storage = "_City")]
        public string City
        {
            get
            {
                return this._City;
            }
            set
            {
                this._City = value;
            }
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
