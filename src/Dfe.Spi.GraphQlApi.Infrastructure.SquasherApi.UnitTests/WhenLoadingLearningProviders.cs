using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.UnitTesting.Fixtures;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Dfe.Spi.Models;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Dfe.Spi.GraphQlApi.Infrastructure.SquasherApi.UnitTests
{
    public class WhenLoadingLearningProviders
    {
        private Mock<IRestClient> _restClientMock;
        private EntityRepositoryConfiguration _configuration;
        private Mock<ILoggerWrapper> _loggerMock;
        private SquasherEntityRepository _repository;
        private CancellationToken _cancellationToken;

        [SetUp]
        public void Arrange()
        {
            _restClientMock = new Mock<IRestClient>();
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<RestRequest>(), _cancellationToken))
                .ReturnsAsync(new RestResponse
                {
                    Content = JsonConvert.SerializeObject(new EntityCollection<LearningProvider>
                    {
                        Entities = new LearningProvider[0],
                    }),
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed,
                });

            _configuration = new EntityRepositoryConfiguration
            {
                SquasherApiBaseUrl = "https://search.example.com/"
            };

            _loggerMock = new Mock<ILoggerWrapper>();

            _repository = new SquasherEntityRepository(
                _restClientMock.Object,
                _configuration,
                _loggerMock.Object);

            _cancellationToken = new CancellationToken();
        }

        [Test, AutoData]
        public async Task ThenItShouldPostSearchRequestToLearningProvidersEndpoint(LoadLearningProvidersRequest request)
        {
            await _repository.LoadLearningProvidersAsync(request, _cancellationToken);

            Func<Parameter, bool> isExpectedBody = (body) =>
                body != null &&
                (string)body.Value == JsonConvert.SerializeObject(request);
            Expression<Func<IRestRequest, bool>> isExpectedRequest = (req) =>
                req.Method == Method.POST &&
                req.Resource == "get-squashed-entity" &&
                isExpectedBody(req.Parameters.SingleOrDefault(p => p.Type == ParameterType.RequestBody));
            _restClientMock.Verify(c => c.ExecuteTaskAsync(It.Is(isExpectedRequest), _cancellationToken),
                Times.Once);
        }
        
        [Test, NonRecursiveAutoData]
        public async Task ThenItShouldReturnDeserializedResponseFromApi(LoadLearningProvidersRequest request,
            EntityCollection<LearningProvider> results)
        {
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<RestRequest>(), _cancellationToken))
                .ReturnsAsync(new RestResponse
                {
                    Content = JsonConvert.SerializeObject(results),
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed,
                });
            
            var actual = await _repository.LoadLearningProvidersAsync(request, _cancellationToken);
            
            Assert.IsNotNull(actual);
            Assert.AreEqual(results.Entities.Length, actual.Entities.Length);
            for (var i = 0; i < results.Entities.Length; i++)
            {
                var expectedEntity = results.Entities[i];
                var actualEntity = actual.Entities[i];
                
                Assert.AreEqual(expectedEntity.Name, actualEntity.Name,
                    $"Expected entity {i} to have Name {expectedEntity.Name} but was {actualEntity.Name}");
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

            var actual = Assert.ThrowsAsync<SquasherApiException>(async () =>
                await _repository.LoadLearningProvidersAsync(new LoadLearningProvidersRequest(), _cancellationToken));
            Assert.AreEqual("get-squashed-entity", actual.Resource);
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
            Assert.AreEqual("Some-details-here", actual.Details);
        }
    }
}