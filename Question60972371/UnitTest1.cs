using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Question60972371
{
    public static class GlobalExceptionLoggerMessage
    {
        public static Action<ILogger, string, Exception> GlobalException = LoggerMessage.Define<string>(
            LogLevel.Error,
            new EventId(StatusCodes.Status500InternalServerError, nameof(GlobalException)),
            "{Message}");

        public static void LogGlobalException(this ILogger logger, string message, Exception exception)
        {
            GlobalException(logger, message, exception);
        }
    }

    public class Tests
    {
        [Test]
        public void LogGlobalExceptionLogTest()
        {
            var loggerMock = new Mock<ILogger<object>>();
            loggerMock.Setup(x => x.IsEnabled(It.IsAny<LogLevel>())).Returns(true);

            var exception = new Exception("Person must exist.");
            const string message = "Person must exist for an object.";

            loggerMock.Object.LogGlobalException(message, exception);

            loggerMock.Verify(x => x.Log(It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }
    }
}