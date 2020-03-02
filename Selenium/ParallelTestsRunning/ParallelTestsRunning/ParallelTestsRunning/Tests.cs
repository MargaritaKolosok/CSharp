using System;
using NUnit.Framework;

namespace ParallelTestsRunning
{
    [TestFixture]
    
    class FireFoxTesting
    {
        [Test]
        public void FireFoxGoogleTest()
        {
            Console.WriteLine("Hello World!");
        }
    }
    [TestFixture]

    class GoogleTesting
    {
        [Test]
        public void ChromeGoogleTest()
        {
            Console.WriteLine("Hello World!");
        }
    }
}
