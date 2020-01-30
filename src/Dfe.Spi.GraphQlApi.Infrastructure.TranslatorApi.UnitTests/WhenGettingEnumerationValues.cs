using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Dfe.Spi.GraphQlApi.Infrastructure.TranslatorApi.UnitTests
{
    public class WhenGettingEnumerationValues
    {
        private EnumerationRepositoryConfiguration _configuration;
        private Mock<IRestClient> _restClientMock;
        private Mock<ILoggerWrapper> _loggerMock;
        private TranslatorApiEnumerationRepository _repository;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Arrange()
        {
            _configuration = new EnumerationRepositoryConfiguration
            {
                TranslatorApiBaseUrl = "https://translator.unit.tests",
            };

            _restClientMock = new Mock<IRestClient>();
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed,
                    Content = JsonConvert.SerializeObject(new GetEnumerationValuesResult
                    {
                        EnumerationValuesResult = new EnumerationValuesResult
                        {
                            EnumerationValues = new string[0],
                        },
                    }),
                });

            _loggerMock = new Mock<ILoggerWrapper>();

            _repository = new TranslatorApiEnumerationRepository(
                _configuration,
                _restClientMock.Object,
                _loggerMock.Object);

            _cancellationToken = new CancellationToken();
        }

        [Test, AutoData]
        public async Task ThenItShouldCallEnumEndpoint(string enumName)
        {
            await _repository.GetEnumerationValuesAsync(enumName, _cancellationToken);

            _restClientMock.Verify(c => c.ExecuteTaskAsync(It.Is<RestRequest>(r =>
                    r.Method == Method.GET &&
                    r.Resource == enumName), _cancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldReturnEnumValues(string enumName, string[] values)
        {
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed,
                    Content = JsonConvert.SerializeObject(new GetEnumerationValuesResult
                    {
                        EnumerationValuesResult = new EnumerationValuesResult
                        {
                            EnumerationValues = values,
                        },
                    }),
                });

            var actual = await _repository.GetEnumerationValuesAsync(enumName, _cancellationToken);

            Assert.IsNotNull(actual);
            Assert.AreEqual(values.Length, actual.Length);
            for (var i = 0; i < values.Length; i++)
            {
                Assert.AreEqual(values[i], actual[i],
                    $"Expected {i} to be {values[i]} but was {actual[i]}");
            }
        }

        [Test, AutoData]
        public void ThenItShouldThrowExceptionIfNonSuccessReturned(string enumName)
        {
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<IRestRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new RestResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ResponseStatus = ResponseStatus.Completed,
                });

            var actual = Assert.ThrowsAsync<TranslatorApiException>(async () =>
                await _repository.GetEnumerationValuesAsync(enumName, _cancellationToken));
            Assert.AreEqual(HttpStatusCode.InternalServerError, actual.StatusCode);
        }
    }
}