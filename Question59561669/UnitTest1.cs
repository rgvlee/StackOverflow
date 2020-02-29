using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Question59561669
{
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var expectedLogMessage = "There is no Customers section in configuration.";
            var services = Mock.Of<IServiceCollection>();
            var configurationMock = new Mock<IConfiguration>();
            var loggerMock = new Mock<ILogger>();

            services.AddCustomersConfiguration(configurationMock.Object, loggerMock.Object);

            configurationMock.Verify(c => c.GetSection("Customers"), Times.Once);
            
            loggerMock.Verify(l => l.Log(
                LogLevel.Error, 
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => ((IReadOnlyList<KeyValuePair<string, object>>)o).Last().Value.ToString().Equals(expectedLogMessage)),
                null, 
                (Func<It.IsAnyType, Exception, string>) It.IsAny<object>()), 
                Times.Once);
        }
    }
}