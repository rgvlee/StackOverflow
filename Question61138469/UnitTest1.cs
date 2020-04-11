//https://stackoverflow.com/a/46172875/2975810

using System;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace Question61138469
{
    public class Example
    {
        public Example(ITestOutputHelper testOutputHelper)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddProvider(new XunitLoggerProvider(testOutputHelper));
            _logger = loggerFactory.CreateLogger<Example>();
        }

        private readonly ILogger<Example> _logger;

        [Fact]
        public void Test()
        {
            _logger.LogDebug("Foo bar baz");
        }
    }

    public class XunitLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public XunitLoggerProvider(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XunitLogger(_testOutputHelper, categoryName);
        }

        public void Dispose() { }
    }

    public class XunitLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly ITestOutputHelper _testOutputHelper;

        public XunitLogger(ITestOutputHelper testOutputHelper, string categoryName)
        {
            _testOutputHelper = testOutputHelper;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _testOutputHelper.WriteLine($"{_categoryName} [{eventId}] {formatter(state, exception)}");
            if (exception != null)
            {
                _testOutputHelper.WriteLine(exception.ToString());
            }
        }

        private class NoopDisposable : IDisposable
        {
            public static readonly NoopDisposable Instance = new NoopDisposable();

            public void Dispose() { }
        }
    }
}