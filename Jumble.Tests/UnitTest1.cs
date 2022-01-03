using System;
using NUnit.Framework;
using Jumble.Util;
namespace Jumble.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PrivateFieldCarryover()
        {
            TestClass testClass = new();
            Console.WriteLine("Setting field value of private field's field (setting a.b.c where a.b is private)");
            testClass.GetFieldValue<TestSubclass>("_subclass").testString = "changed";
            Console.WriteLine("a.b.c changed, old a.b.c is \"unchanged\"");
            Console.WriteLine("new a.b.c = "+testClass.GetTestString());
            Assert.AreEqual("changed", testClass.GetTestString());
        }
    }    
}

