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
using Dfe.Spi.Models.Entities;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;

namespace Dfe.Spi.GraphQlApi.Infrastructure.SquasherApi.UnitTests
{
    public class WhenLoadingManagementGroups
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
                    Content = JsonConvert.SerializeObject(new EntityCollection<ManagementGroup>
                    {
                        SquashedEntityResults = new SquashedEntityResult<ManagementGroup>[0],
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
        public async Task ThenItShouldPostSearchRequestToManagementGroupsEndpoint(LoadManagementGroupsRequest request)
        {
            request.AggregatesRequest = null;
            
            await _repository.LoadManagementGroupsAsync(request, _cancellationToken);

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
        public async Task ThenItShouldReturnDeserializedResponseFromApi(LoadManagementGroupsRequest request,
            EntityCollection<ManagementGroup> results)
        {
            _restClientMock.Setup(c => c.ExecuteTaskAsync(It.IsAny<RestRequest>(), _cancellationToken))
                .ReturnsAsync(new RestResponse
                {
                    Content = JsonConvert.SerializeObject(results),
                    StatusCode = HttpStatusCode.OK,
                    ResponseStatus = ResponseStatus.Completed,
                });

            var actual = await _repository.LoadManagementGroupsAsync(request, _cancellationToken);

            Assert.IsNotNull(actual);
            Assert.AreEqual(results.SquashedEntityResults.Length, actual.SquashedEntityResults.Length);
            for (var i = 0; i < results.SquashedEntityResults.Length; i++)
            {
                var expectedEntity = results.SquashedEntityResults[i].SquashedEntity;
                var actualEntity = actual.SquashedEntityResults[i].SquashedEntity;

                Assert.AreEqual(expectedEntity.Name, actualEntity.Name,
                    $"Expected entity {i} to have Name {expectedEntity.Name} but was {actualEntity.Name}");
                Assert.AreEqual(expectedEntity.Type, actualEntity.Type,
                    $"Expected entity {i} to have Type {expectedEntity.Type} but was {actualEntity.Type}");
                Assert.AreEqual(expectedEntity.Identifier, actualEntity.Identifier,
                    $"Expected entity {i} to have Identifier {expectedEntity.Identifier} but was {actualEntity.Identifier}");
                Assert.AreEqual(expectedEntity.Code, actualEntity.Code,
                    $"Expected entity {i} to have Code {expectedEntity.Code} but was {actualEntity.Code}");
                Assert.AreEqual(expectedEntity.CompaniesHouseNumber, actualEntity.CompaniesHouseNumber,
                    $"Expected entity {i} to have CompaniesHouseNumber {expectedEntity.CompaniesHouseNumber} but was {actualEntity.CompaniesHouseNumber}");
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
                await _repository.LoadManagementGroupsAsync(new LoadManagementGroupsRequest(), _cancellationToken));
            Assert.AreEqual("get-squashed-entity", actual.Resource);
            Assert.AreEqual(HttpStatusCode.BadRequest, actual.Status);
            Assert.AreEqual("Some-details-here", actual.Details);
        }
    }
}