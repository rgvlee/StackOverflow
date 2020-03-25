using System;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Question60842761
{
    public class ObjectToReturn
    {
        public Guid Id { get; set; }
    }

    public interface IFoo
    {
        ObjectToReturn MyMethod(int parameter1);

        ObjectToReturn MyMethod(string parameter2);

        ObjectToReturn MyMethod(int parameter1, int parameter2);

        ObjectToReturn AnotherMethod();

        int AValueTypeMethod();
    }

    public class SelectiveDefaultValueProvider : DefaultValueProvider
    {
        private readonly string _methodName;
        private readonly object _returns;

        public SelectiveDefaultValueProvider(string methodName, object returns)
        {
            _methodName = methodName;
            _returns = returns;
        }

        protected override object GetDefaultValue(Type type, Mock mock)
        {
            var lastInvocation = mock.Invocations.Last();
            var methodInfo = lastInvocation.Method;
            var args = lastInvocation.Arguments;

            if (methodInfo.Name.Equals(_methodName))
            {
                return _returns;
            }

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }

    public class Tests
    {
        [Test]
        public void DefaultValueProvider_ForOverloadedMethod_AllOverloadsReturnSameExpectedResult()
        {
            var objectToReturn = new ObjectToReturn { Id = Guid.NewGuid() };
            var mock = new Mock<IFoo> { DefaultValueProvider = new SelectiveDefaultValueProvider(nameof(IFoo.MyMethod), objectToReturn) };
            var mocked = mock.Object;

            var result1 = mocked.MyMethod(1);
            var result2 = mocked.MyMethod(1, 2);
            var result3 = mocked.MyMethod("asdf");
            var result4 = mocked.AnotherMethod();
            var result5 = mocked.AValueTypeMethod();

            Assert.Multiple(() =>
            {
                Assert.That(result1, Is.SameAs(objectToReturn));
                Assert.That(result2, Is.SameAs(objectToReturn));
                Assert.That(result3, Is.SameAs(objectToReturn));
                Assert.That(result4, Is.Null);
                Assert.That(result5, Is.TypeOf<int>());
            });
        }
    }
}