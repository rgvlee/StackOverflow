using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using NUnit.Framework;

namespace Question60684578
{
    public class MyClass
    {
        private readonly ILogger<MyClass> _logger;

        public MyClass(ILogger<MyClass> logger)
        {
            _logger = logger;
        }

        public void MyMethod(string message)
        {
            _logger.LogError(message);
        }
    }

    public class TestsUsingMoq
    {
        [Test]
        public void MyMethod_String_LogsError()
        {
            // Arrange
            var logger = Mock.Of<ILogger<MyClass>>();

            var myClass = new MyClass(logger);

            var message = "a message";

            // Act
            myClass.MyMethod(message);

            //Assert
            Mock.Get(logger)
                .Verify(l => l.Log(LogLevel.Error,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((o, t) => ((IReadOnlyList<KeyValuePair<string, object>>) o).Last().Value.ToString().Equals(message)),
                        It.IsAny<Exception>(),
                        (Func<It.IsAnyType, Exception, string>) It.IsAny<object>()),
                    Times.Once);
        }
    }

    public class TestsUsingNSubstitute
    {
        [Test]
        public void MyMethod_String_LogsError()
        {
            // Arrange
            var logger = Substitute.For<ILogger<MyClass>>();

            var myClass = new MyClass(logger);

            var message = "a message";

            // Act
            myClass.MyMethod(message);

            //Assert
            Assert.That(logger.ReceivedCalls()
                    .Select(call => call.GetArguments())
                    .Count(callArguments => ((LogLevel) callArguments[0]).Equals(LogLevel.Error) &&
                                            ((IReadOnlyList<KeyValuePair<string, object>>) callArguments[2]).Last().Value.ToString().Equals(message)),
                Is.EqualTo(1));
        }
    }
}