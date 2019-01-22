using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vEventsFiddle6
{
    class ThresholdReachedEventArgs : EventArgs
    {
        public int Threshold
        {
            get;
            set;
        }
        public DateTime TimeReached
        {
            get;
            set;
        }
    }

    

    class Counter
    {
       private int threshold;
       private int total;

        public Counter(int threshold)
        {
            this.threshold = threshold;
        }

        public void Add(int x)
        {
            total += x;
            if (total >= threshold)
            {
                ThresholdReachedEventArgs args = new ThresholdReachedEventArgs();
                args.Threshold = threshold;
                args.TimeReached = DateTime.Now;

            }
        }
        protected virtual void OnThresholdReached(ThresholdReachedEventArgs e)
        {

        }

        public event EventHandler<ThresholdReachedEventArgs> ThresholdReached;
    }
    

    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
