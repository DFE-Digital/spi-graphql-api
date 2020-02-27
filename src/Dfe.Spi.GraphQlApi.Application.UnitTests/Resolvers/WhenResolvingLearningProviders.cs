using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.UnitTesting.Fixtures;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Dfe.Spi.Models.Entities;
using GraphQL.Types;
using Moq;
using NUnit.Framework;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests.Resolvers
{
    public class WhenResolvingLearningProviders
    {
        private Mock<IEntityRepository> _entityRepositoryMock;
        private Mock<ILoggerWrapper> _loggerMock;
        private Mock<IEntityReferenceBuilder> _entityReferenceBuilderMock;
        private LearningProvidersResolver _resolver;

        [SetUp]
        public void Arrange()
        {
            _entityRepositoryMock = new Mock<IEntityRepository>();
            _entityRepositoryMock.Setup(r =>
                    r.LoadLearningProvidersAsync(It.IsAny<LoadLearningProvidersRequest>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityCollection<LearningProvider>
                {
                    SquashedEntityResults = new SquashedEntityResult<LearningProvider>[0],
                });
            
            _loggerMock = new Mock<ILoggerWrapper>();
            
            _entityReferenceBuilderMock = new Mock<IEntityReferenceBuilder>();
            _entityReferenceBuilderMock.Setup(b =>
                    b.GetEntityReferences(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AggregateEntityReference[0]);

            _resolver = new LearningProvidersResolver(
                _entityRepositoryMock.Object,
                _loggerMock.Object,
                _entityReferenceBuilderMock.Object);
        }

        [Test]
        public async Task ThenItShouldReturnArrayOfLearningProviders()
        {
            var context = BuildResolveFieldContext();

            var actual = await _resolver.ResolveAsync(context);

            Assert.IsNotNull(actual);
        }

        [Test, AutoData]
        public async Task ThenItShouldUseNameArgAsSearchCriteria(string name)
        {
            var context = BuildResolveFieldContext(name);

            await _resolver.ResolveAsync(context);

            _entityReferenceBuilderMock.Verify(
                p => p.GetEntityReferences(It.Is<SearchRequest>(r => IsSearchRequestWithNameFilter(r, name)),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldUseBuiltEntityReferencesToLoadData(AggregateEntityReference[] entityReferences)
        {
            _entityReferenceBuilderMock.Setup(b =>
                    b.GetEntityReferences(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entityReferences);
            var context = BuildResolveFieldContext();
            
            await _resolver.ResolveAsync(context);
            
            _entityRepositoryMock.Verify(r=>r.LoadLearningProvidersAsync(
                It.Is<LoadLearningProvidersRequest>(req=>req.EntityReferences == entityReferences),
                context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldRequestFieldsFromGraphQuery(AggregateEntityReference[] entityReferences)
        {
            _entityReferenceBuilderMock.Setup(b =>
                    b.GetEntityReferences(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entityReferences);
            var fields = new[] {"name", "urn", "ukprn"};
            var context = BuildResolveFieldContext(fields: fields);
            
            await _resolver.ResolveAsync(context);
            
            _entityRepositoryMock.Verify(r=>r.LoadLearningProvidersAsync(
                It.Is<LoadLearningProvidersRequest>(req=>
                    req.Fields != null &&
                    req.Fields.Length == 3 &&
                    req.Fields.Contains("urn") &&
                    req.Fields.Contains("ukprn") &&
                    req.Fields.Contains("name")),
                context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldAlwaysUseUrnAndUkprnInFieldsWhenGettingReferencedEntities(AggregateEntityReference[] entityReferences)
        {
            _entityReferenceBuilderMock.Setup(b =>
                    b.GetEntityReferences(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(entityReferences);
            var fields = new[] {"name", "postcode"};
            var context = BuildResolveFieldContext(fields: fields);
            
            await _resolver.ResolveAsync(context);
            
            _entityRepositoryMock.Verify(r=>r.LoadLearningProvidersAsync(
                It.Is<LoadLearningProvidersRequest>(req=>
                    req.Fields != null &&
                    req.Fields.Length == 4 &&
                    req.Fields.Contains("urn") &&
                    req.Fields.Contains("ukprn") &&
                    req.Fields.Contains("name") &&
                    req.Fields.Contains("postcode")),
                context.CancellationToken),
                Times.Once);
        }

        [Test, NonRecursiveAutoData]
        public async Task ThenItShouldReturnEntitiesFromRepo(LearningProvider[] entities)
        {
            _entityRepositoryMock.Setup(r =>
                    r.LoadLearningProvidersAsync(It.IsAny<LoadLearningProvidersRequest>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityCollection<LearningProvider>
                {
                    SquashedEntityResults = entities.Select(e =>
                        new SquashedEntityResult<LearningProvider>
                        {
                            SquashedEntity = e,
                        }).ToArray(),
                });
            var context = BuildResolveFieldContext();
        
            var actual = await _resolver.ResolveAsync(context);
        
            Assert.AreEqual(entities.Length, actual.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.AreSame(entities[i], actual[i],
                    $"Expected {i} to be {entities[i]} but was {actual[i]}");
            }
        }


        private ResolveFieldContext<object> BuildResolveFieldContext(string name = null, string[] fields = null)
        {
            var groups = new List<object>
            {
                new Dictionary<string, object>
                {
                    {"isAnd", true},
                    {
                        "conditions", new List<object>
                        {
                            new Dictionary<string, object>
                            {
                                {"field", "Name"},
                                {"operator", "equals"},
                                {"value", name ?? Guid.NewGuid().ToString()},
                            }
                        }
                    },
                }
            };
            var criteria = new Dictionary<string, object>
            {
                { "isAnd", true },
                { "groups", groups }
            };
            return TestHelper.BuildResolveFieldContext<object>(arguments: new Dictionary<string, object>
            {
                {"criteria", criteria},
            }, fields: fields);
        }

        private bool IsSearchRequestWithNameFilter(SearchRequest searchRequest, string name)
        {
            return searchRequest.Filter.Any(f => f.Field == "Name" && f.Value == name);
        }
    }
}