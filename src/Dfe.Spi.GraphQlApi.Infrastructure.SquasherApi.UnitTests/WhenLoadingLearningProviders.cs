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
        private Mock<ISpiExecutionContextManager> _executionContextManager;
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
                        SquashedEntityResults = new SquashedEntityResult<LearningProvider>[0],
                    }),
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed,
                });

            _configuration = new EntityRepositoryConfiguration
            {
                SquasherApiBaseUrl = "https://search.example.com/"
            };
            
            _executionContextManager = new Mock<ISpiExecutionContextManager>();
            _executionContextManager.Setup(m => m.SpiExecutionContext)
                .Returns(new SpiExecutionContext());

            _loggerMock = new Mock<ILoggerWrapper>();

            _repository = new SquasherEntityRepository(
                _restClientMock.Object,
                _configuration,
                _executionContextManager.Object,
                _loggerMock.Object);

            _cancellationToken = new CancellationToken();
        }

        [Test, AutoData]
        public async Task ThenItShouldPostSearchRequestToLearningProvidersEndpoint(LoadLearningProvidersRequest request)
        {
            await _repository.LoadLearningProvidersAsync(request, _cancellationToken);

            var expectedTranslatedRequest = new GetSquashedEntitiesRequest
            {
                EntityName = request.EntityName,
                EntityReferences = request.EntityReferences.Select(er =>
                    new SquasherEntityReference
                    {
                        AdapterRecordReferences = er.AdapterRecordReferences.Select(arr=>
                            new SquasherAdapterReference
                            {
                                Source = arr.SourceSystemName,
                                Id = arr.SourceSystemId,
                            }).ToArray(),
                    }).ToArray(),
                Fields = request.Fields,
            };
            Func<Parameter, bool> isExpectedBody = (body) =>
                body != null &&
                (string) body.Value == JsonConvert.SerializeObject(expectedTranslatedRequest);
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
            Assert.AreEqual(results.SquashedEntityResults.Length, actual.SquashedEntityResults.Length);
            for (var i = 0; i < results.SquashedEntityResults.Length; i++)
            {
                var expectedEntity = results.SquashedEntityResults[i].SquashedEntity;
                var actualEntity = actual.SquashedEntityResults[i].SquashedEntity;

                Assert.AreEqual(expectedEntity.Name, actualEntity.Name,
                    $"Expected entity {i} to have Name {expectedEntity.Name} but was {actualEntity.Name}");
                Assert.AreEqual(expectedEntity.LegalName, actualEntity.LegalName,
                    $"Expected entity {i} to have LegalName {expectedEntity.LegalName} but was {actualEntity.LegalName}");
                Assert.AreEqual(expectedEntity.Urn, actualEntity.Urn,
                    $"Expected entity {i} to have Urn {expectedEntity.Urn} but was {actualEntity.Urn}");
                Assert.AreEqual(expectedEntity.Ukprn, actualEntity.Ukprn,
                    $"Expected entity {i} to have Ukprn {expectedEntity.Ukprn} but was {actualEntity.Ukprn}");
                Assert.AreEqual(expectedEntity.Postcode, actualEntity.Postcode,
                    $"Expected entity {i} to have Postcode {expectedEntity.Postcode} but was {actualEntity.Postcode}");
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