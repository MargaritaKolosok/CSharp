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
       private int total=0;

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
            EventHandler<ThresholdReachedEventArgs> handler = ThresholdReached;
            ThresholdReached?.Invoke(this, e);
        }

        public event EventHandler<ThresholdReachedEventArgs> ThresholdReached;
    }
   
    class Program
    {
        static void MethodThresholdReached(object sender, ThresholdReachedEventArgs e)
        {
            Console.WriteLine(sender);
            Console.WriteLine($"{e.Threshold} was reached at {e.TimeReached}");
            Environment.Exit(0);
        }

        static void Main(string[] args)
        {
            Counter counter = new Counter(5);
            
            counter.ThresholdReached += MethodThresholdReached;

            Console.WriteLine("Press a to continue and incrice num");
            while (Console.ReadKey(true).KeyChar =='a')
            {
                Console.WriteLine("Adding 1");
                counter.Add(1);
            }


            Console.ReadKey();

        }
    }
}
