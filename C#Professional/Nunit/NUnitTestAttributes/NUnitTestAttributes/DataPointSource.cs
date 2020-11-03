using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NUnitTestAttributes
{
    public class DataPointSource
    {
        [DatapointSource]
        public double[] values = new double[] { 0.0, 1.0, -1.0, 42.0 };

        [DatapointSource]
        public double[] values2 = new double[] { 0.3, 1.3, -1.3, 45.0 };

        [Theory]
        public void SquareRootDefinition(double num)
        {
            Assume.That(num >= 0.0);

            double sqrt = Math.Sqrt(num);
            Console.WriteLine("num " + num);
            Assert.That(sqrt >= 0.0);
            Assert.That(sqrt * sqrt, Is.EqualTo(num).Within(0.000001));
        }

    }
}
