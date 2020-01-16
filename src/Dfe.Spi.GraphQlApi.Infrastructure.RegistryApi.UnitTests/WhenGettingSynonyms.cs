using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Dfe.Spi.GraphQlApi.Infrastructure.RegistryApi.UnitTests
{
    public class WhenGettingSynonyms
    {
        private Mock<IRestClient> _restClientMock;
        private RegistryConfiguration _configuration;
        private Mock<ILoggerWrapper> _loggerMock;
        private RegistryApiRegistryProvider _provider;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Arrange()
        {
            _restClientMock = new Mock<IRestClient>();
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<RestRequest>(), _cancellationToken))
                .ReturnsAsync(new RestResponse
                {
                    Content = JsonConvert.SerializeObject(new GetSynonymsResult
                    {
                        Synonyms = new EntityReference[0],
                    }),
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed,
                });

            _configuration = new RegistryConfiguration
            {
                RegistryApiBaseUrl = "https://search.example.com/",
            };

            _loggerMock = new Mock<ILoggerWrapper>();

            _provider = new RegistryApiRegistryProvider(
                _restClientMock.Object,
                _configuration,
                _loggerMock.Object);

            _cancellationToken = new CancellationToken();
        }

        [Test, AutoData]
        public async Task ThenItShouldRequestSynonymsFromApi(string entityType, string sourceSystem,
            string sourceSystemId)
        {
            await _provider.GetSynonymsAsync(entityType, sourceSystem, sourceSystemId, _cancellationToken);

            _restClientMock.Verify(c => c.ExecuteTaskAsync(It.Is<RestRequest>(req =>
                    req.Method == Method.GET &&
                    req.Resource == $"{entityType}/{sourceSystem}/{sourceSystemId}/synonyms"), _cancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldReturnEntityReferencesFromResult(string entityType, string sourceSystem,
            string sourceSystemId,
            EntityReference[] synonyms)
        {
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<RestRequest>(), _cancellationToken))
                .ReturnsAsync(new RestResponse
                {
                    Content = JsonConvert.SerializeObject(new GetSynonymsResult
                    {
                        Synonyms = synonyms,
                    }),
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed,
                });

            var actual = await _provider.GetSynonymsAsync(entityType, sourceSystem, sourceSystemId, _cancellationToken);

            Assert.IsNotNull(actual);
            Assert.AreEqual(synonyms.Length, actual.Length);
            for (var i = 0; i < synonyms.Length; i++)
            {
                Assert.AreEqual(synonyms[i].SourceSystemName, actual[i].SourceSystemName,
                    $"Expected item [{i}].SourceSystemName to be {synonyms[i]} but was {actual[i]}");
                Assert.AreEqual(synonyms[i].SourceSystemId, actual[i].SourceSystemId,
                    $"Expected item [{i}].SourceSystemId to be {synonyms[i]} but was {actual[i]}");
            }
        }
    }
}