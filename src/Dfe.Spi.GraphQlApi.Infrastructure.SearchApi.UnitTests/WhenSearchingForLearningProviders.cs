using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.Common.Context.Definitions;
using Dfe.Spi.Common.Context.Models;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Dfe.Spi.GraphQlApi.Infrastructure.SearchApi.UnitTests
{
    public class WhenSearchingForLearningProviders
    {
        private Mock<IRestClient> _restClientMock;
        private SearchConfiguration _configuration;
        private Mock<ISpiExecutionContextManager> _executionContextManager;
        private Mock<ILoggerWrapper> _loggerMock;
        private SearchApiSearchProvider _provider;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Arrange()
        {
            _restClientMock = new Mock<IRestClient>();
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<RestRequest>(), _cancellationToken))
                .ReturnsAsync(new RestResponse
                {
                    Content = JsonConvert.SerializeObject(new SearchResultSet<LearningProviderReference>
                    {
                        Documents = new LearningProviderReference[0],
                    }),
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed,
                });

            _configuration = new SearchConfiguration
            {
                SearchApiBaseUrl = "https://search.example.com/"
            };
            
            _executionContextManager = new Mock<ISpiExecutionContextManager>();
            _executionContextManager.Setup(m => m.SpiExecutionContext)
                .Returns(new SpiExecutionContext());

            _loggerMock = new Mock<ILoggerWrapper>();

            _provider = new SearchApiSearchProvider(
                _restClientMock.Object,
                _configuration,
                _executionContextManager.Object,
                _loggerMock.Object);

            _cancellationToken = new CancellationToken();
        }

        [Test, AutoData]
        public async Task ThenItShouldPostSearchRequestToLearningProvidersEndpoint(SearchRequest request)
        {
            await _provider.SearchLearningProvidersAsync(request, _cancellationToken);

            Func<Parameter, bool> isExpectedBody = (body) =>
                body != null &&
                (string)body.Value == JsonConvert.SerializeObject(request);
            Expression<Func<IRestRequest, bool>> isExpectedRequest = (req) =>
                req.Method == Method.POST &&
                req.Resource == "learning-providers" &&
                isExpectedBody(req.Parameters.SingleOrDefault(p => p.Type == ParameterType.RequestBody));
            _restClientMock.Verify(c => c.ExecuteTaskAsync(It.Is(isExpectedRequest), _cancellationToken),
                Times.Once);
        }
        
        [Test, AutoData]
        public async Task ThenItShouldReturnDeserializedResponseFromApi(SearchRequest request,
            SearchResultSet<LearningProviderReference> results)
        {
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<RestRequest>(), _cancellationToken))
                .ReturnsAsync(new RestResponse
                {
                    Content = JsonConvert.SerializeObject(results),
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed,
                });
            
            var actual = await _provider.SearchLearningProvidersAsync(request, _cancellationToken);
            
            Assert.IsNotNull(actual);
            Assert.AreEqual(results.Documents.Length, actual.Documents.Length);
            for (var i = 0; i < results.Documents.Length; i++)
            {
                var expectedDocument = results.Documents[i];
                var actualDocument = actual.Documents[i];
                
                Assert.AreEqual(expectedDocument.SourceSystemName, actualDocument.SourceSystemName,
                    $"Expected document {i} to have SourceSystemName {expectedDocument.SourceSystemName} but was {actualDocument.SourceSystemName}");
                Assert.AreEqual(expectedDocument.SourceSystemId, actualDocument.SourceSystemId,
                    $"Expected document {i} to have SourceSystemId {expectedDocument.SourceSystemId} but was {actualDocument.SourceSystemId}");
            }
        }

        [Test]
        public async Task ThenItShouldThrowExceptionIfApiRequestNotSuccessful()
        {
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<RestRequest>(), _cancellationToken))
                .ReturnsAsync(new RestResponse
                {
                    Content = "Some-details-here",
                    StatusCode = HttpStatusCode.BadRequest,
                    ResponseStatus = ResponseStatus.Completed,
                });

            var actual = Assert.ThrowsAsync<SearchApiException>(async () =>
                await _provider.SearchLearningProvidersAsync(new SearchRequest(), _cancellationToken));
            Assert.AreEqual("learning-providers", actual.Resource);
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
            Assert.AreEqual("Some-details-here", actual.Details);
        }
    }
}