using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Question59741796
{
    public class Tests
    {
        [Test]
        public void SpotifyClientInformation_ReturnsExpectedClientIdAndSecretIdTuple()
        {
            var logger = Mock.Of<ILogger<Helper>>();

            var expectedClientId = "My client Id";
            var expectedSecretId = "My secret Id";
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(m => m["SpotifySecrets:clientID"]).Returns(expectedClientId);
            configurationMock.Setup(m => m["SpotifySecrets:secretID"]).Returns(expectedSecretId);
            var configuration = configurationMock.Object;

            var helper = new Helper(configuration, logger);

            var (actualClientId, actualSecretId) = helper.SpotifyClientInformation();

            Assert.Multiple(() =>
            {
                Assert.That(actualClientId, Is.EqualTo(expectedClientId));
                Assert.That(actualSecretId, Is.EqualTo(expectedSecretId));
            });
        }

        [Test]
        public void SpotifyClientInformation_WithNullConfiguration_ReturnsNull()
        {
            var logger = Mock.Of<ILogger<Helper>>();
            var helper = new Helper(null, logger);

            var actualResult = helper.SpotifyClientInformation();

            Assert.That(actualResult, Is.Null);
        }

        [Test]
        public void Initialize_NullConfiguration_GeneratesCriticalLog()
        {
            var expectedLogMessage = "Configuration DI not set up correctly";

            var logger = Mock.Of<ILogger<Helper>>();

            var helper = new Helper(null, logger);
            
            Mock.Get(logger).Verify(m => m.Log(
                    LogLevel.Critical,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => HasLogMessage(o, expectedLogMessage)),
                    It.IsAny<KeyNotFoundException>(),
                    (Func<It.IsAnyType, Exception, string>) It.IsAny<object>()),
                Times.Once
            );
        }

        private static bool HasLogMessage(object state, string expectedMessage)
        {
            var loggedValues = (IReadOnlyList<KeyValuePair<string, object>>) state;
            var unformattedMessage = loggedValues[^1].Value.ToString();
            return unformattedMessage.Equals(expectedMessage, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}