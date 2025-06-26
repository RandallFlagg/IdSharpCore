using System;

using IdSharp.Common.Events;

using NUnit.Framework;

namespace IdSharp.Common.Tests.Events
{
    [TestFixture]
    public class DataEventArgsTest
    {
        [Test]
        public void TestConstructorAndDataProperty_Null()
        {
            DataEventArgs<string> dea = new DataEventArgs<string>(null);
            Assert.That(dea.Data, Is.Null);
        }

        [Test]
        public void TestConstructorAndDataProperty_NotNull()
        {
            DataEventArgs<int> dea = new DataEventArgs<int>(42);
            Assert.That(42, Is.EqualTo(dea.Data));
        }

        [Test]
        public void TestSetData_Null()
        {
            var dea = new DataEventArgs<string>("Hello");
            Assert.That("Hello", Is.EqualTo(dea.Data));
            dea.Data = null;
            Assert.That(dea.Data, Is.Null);
        }

        [Test]
        public void TestSetData_NotNull()
        {
            var dea = new DataEventArgs<string>("Hello");
            Assert.That("Hello", Is.EqualTo(dea.Data));
            dea.Data = "World";
            Assert.That("World", Is.EqualTo(dea.Data));
        }
    }
}
