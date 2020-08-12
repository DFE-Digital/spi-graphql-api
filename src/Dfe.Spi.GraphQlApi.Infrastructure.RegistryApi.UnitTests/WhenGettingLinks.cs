using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.Common.Context.Definitions;
using Dfe.Spi.Common.Context.Models;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Dfe.Spi.GraphQlApi.Infrastructure.RegistryApi.UnitTests
{
    public class WhenGettingLinks
    {
        private Mock<IRestClient> _restClientMock;
        private RegistryConfiguration _configuration;
        private Mock<ISpiExecutionContextManager> _executionContextManager;
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
            
            _executionContextManager = new Mock<ISpiExecutionContextManager>();
            _executionContextManager.Setup(m => m.SpiExecutionContext)
                .Returns(new SpiExecutionContext());

            _loggerMock = new Mock<ILoggerWrapper>();

            _provider = new RegistryApiRegistryProvider(
                _restClientMock.Object,
                _configuration,
                _executionContextManager.Object,
                _loggerMock.Object);

            _cancellationToken = new CancellationToken();
        }

        [Test, AutoData]
        public async Task ThenItShouldRequestLinksFromApi(string entityType, string sourceSystem, string sourceSystemId)
        {
            await _provider.GetLinksAsync(entityType, sourceSystem, sourceSystemId, null, _cancellationToken);

            _restClientMock.Verify(c => c.ExecuteTaskAsync(It.Is<RestRequest>(req =>
                    req.Method == Method.GET &&
                    req.Resource == $"{entityType}/{sourceSystem}/{sourceSystemId}/links"), _cancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldReturnEntityReferenceLinksFromResult(string entityType, string sourceSystem, string sourceSystemId, 
            EntityLinkReference[] links)
        {
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<RestRequest>(), _cancellationToken))
                .ReturnsAsync(new RestResponse
                {
                    Content = JsonConvert.SerializeObject(new GetLinksResult
                    {
                        Links = links,
                    }),
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed,
                });

            var actual = await _provider.GetLinksAsync(entityType, sourceSystem, sourceSystemId, null, _cancellationToken);

            Assert.IsNotNull(actual);
            Assert.AreEqual(links.Length, actual.Length);
            for (var i = 0; i < links.Length; i++)
            {
                Assert.AreEqual(links[i].SourceSystemName, actual[i].SourceSystemName,
                    $"Expected item [{i}].SourceSystemName to be {links[i].SourceSystemName} but was {actual[i].SourceSystemName}");
                Assert.AreEqual(links[i].SourceSystemId, actual[i].SourceSystemId,
                    $"Expected item [{i}].SourceSystemId to be {links[i].SourceSystemId} but was {actual[i].SourceSystemId}");
                Assert.AreEqual(links[i].LinkType, actual[i].LinkType,
                    $"Expected item [{i}].LinkType to be {links[i].LinkType} but was {actual[i].LinkType}");
            }
        }

        [Test, AutoData]
        public async Task ThenItShouldReturnEmptyArrayIfNotFoundReturned(string entityType, string sourceSystem,
            string sourceSystemId)
        {
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<RestRequest>(), _cancellationToken))
                .ReturnsAsync(new RestResponse
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ResponseStatus = ResponseStatus.Completed,
                });

            var actual = await _provider.GetLinksAsync(entityType, sourceSystem, sourceSystemId, null, _cancellationToken);

            Assert.IsNotNull(actual);
            Assert.IsEmpty(actual);
        }

        [Test, AutoData]
        public void ThenItShouldThrowExceptionIfNonSuccessReturned(string entityType, string sourceSystem,
            string sourceSystemId)
        {
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<RestRequest>(), _cancellationToken))
                .ReturnsAsync(new RestResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ResponseStatus = ResponseStatus.Completed,
                });

            Assert.ThrowsAsync<RegistryApiException>(async () =>
                await _provider.GetLinksAsync(entityType, sourceSystem, sourceSystemId, null, _cancellationToken));
        }
    }
}