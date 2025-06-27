using IdSharp.Common.Events;
using NUnit.Framework;

namespace IdSharp.Common.Tests.Events
{
    [TestFixture]
    public class InvalidDataEventArgsTest
    {
        [Test]
        public void Test_Ctors()
        {
            var x = new InvalidDataEventArgs("propertyName", "message");
            Assert.That("propertyName", Is.EqualTo(x.Property));
            Assert.That("message", Is.EqualTo(x.Message));
            Assert.That(ErrorType.Warning, Is.EqualTo(x.ErrorType));

            x = new InvalidDataEventArgs("propertyName2", "message", ErrorType.Error);
            Assert.That("propertyName2", Is.EqualTo(x.Property));
            Assert.That("message", Is.EqualTo(x.Message));
            Assert.That(ErrorType.Error, Is.EqualTo(x.ErrorType));

            x = new InvalidDataEventArgs("propertyName3", ErrorType.None);
            Assert.That("propertyName3", Is.EqualTo(x.Property));
            Assert.That(x.Message, Is.Null.Or.Empty);
            Assert.That(ErrorType.None, Is.EqualTo(x.ErrorType));
        }
    }
}
