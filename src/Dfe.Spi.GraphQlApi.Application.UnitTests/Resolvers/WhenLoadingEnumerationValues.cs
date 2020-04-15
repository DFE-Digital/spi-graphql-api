using System.Threading;
using AutoFixture.NUnit3;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using Dfe.Spi.GraphQlApi.Domain.Enumerations;
using Moq;
using NUnit.Framework;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests.Resolvers
{
    public class WhenLoadingEnumerationValues
    {
        private Mock<IEnumerationRepository> _enumerationRepositoryMock;
        private EnumerationLoader _loader;

        [SetUp]
        public void Arrange()
        {
            _enumerationRepositoryMock = new Mock<IEnumerationRepository>();
            _enumerationRepositoryMock.Setup(r =>
                    r.GetEnumerationValuesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new string[0]);

            _loader = new EnumerationLoader(_enumerationRepositoryMock.Object);
        }

        [Test, AutoData]
        public void ThenItShouldGetEnumValuesFromRepository(string enumName)
        {
            _loader.GetEnumerationValues(enumName);

            _enumerationRepositoryMock.Verify(r => r.GetEnumerationValuesAsync(enumName, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test, AutoData]
        public void ThenItShouldReturnValues(string enumName, string[] values)
        {
            _enumerationRepositoryMock.Setup(r =>
                    r.GetEnumerationValuesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(values);

            var actual = _loader.GetEnumerationValues(enumName);

            Assert.IsNotNull(actual);
            Assert.AreEqual(values.Length, actual.Length);
            for (var i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(values[i], actual[i].Value,
                    $"Expected {i} to have value {values[i]} but got {actual[i].Value}");
            }
        }
    }
}