using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.UnitTesting.Fixtures;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Context;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.Models.Entities;
using GraphQL.Language.AST;
using GraphQL.Types;
using Moq;
using NUnit.Framework;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests.Resolvers
{
    public class WhenResolvingLearningProviders
    {
        private Mock<IEntityRepository> _entityRepositoryMock;
        private Mock<IRegistryProvider> _registryProviderMock;
        private Mock<IGraphExecutionContextManager> _executionContextManagerMock;
        private Mock<ILoggerWrapper> _loggerMock;
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

            _registryProviderMock = new Mock<IRegistryProvider>();
            _registryProviderMock.Setup(r =>
                    r.SearchLearningProvidersAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResultSet
                {
                    Results = new[]
                    {
                        new SearchResult
                        {
                            Entities = new EntityReference[0],
                        },
                    }
                });
            
            _executionContextManagerMock = new Mock<IGraphExecutionContextManager>();
            _executionContextManagerMock.Setup(m => m.GraphExecutionContext)
                .Returns(new GraphExecutionContext());

            _loggerMock = new Mock<ILoggerWrapper>();

            _resolver = new LearningProvidersResolver(
                _entityRepositoryMock.Object,
                _registryProviderMock.Object,
                _executionContextManagerMock.Object,
                _loggerMock.Object);
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

            _registryProviderMock.Verify(b => b.SearchLearningProvidersAsync(
                    It.Is<SearchRequest>(r =>
                        r.Groups != null &&
                        r.Groups.Length == 1 &&
                        r.Groups[0].Filter != null &&
                        r.Groups[0].Filter.Length == 1 &&
                        r.Groups[0].Filter[0].Field == "Name" &&
                        r.Groups[0].Filter[0].Value == name),
                    context.CancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldDefaultToSkipZeroIfNotSpecified()
        {
            var context = BuildResolveFieldContext();

            await _resolver.ResolveAsync(context);

            _registryProviderMock.Verify(b => b.SearchLearningProvidersAsync(
                    It.Is<SearchRequest>(r => r.Skip == 0),
                    context.CancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldUseSkipArgumentIfSpecified()
        {
            var context = BuildResolveFieldContext(skip: 25);

            await _resolver.ResolveAsync(context);

            _registryProviderMock.Verify(b => b.SearchLearningProvidersAsync(
                    It.Is<SearchRequest>(r => r.Skip == 25),
                    context.CancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldDefaultToTake50IfNotSpecified()
        {
            var context = BuildResolveFieldContext();

            await _resolver.ResolveAsync(context);

            _registryProviderMock.Verify(b => b.SearchLearningProvidersAsync(
                    It.Is<SearchRequest>(r => r.Take == 50),
                    context.CancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldUseTakeArgumentIfSpecified()
        {
            var context = BuildResolveFieldContext(take: 36);

            await _resolver.ResolveAsync(context);

            _registryProviderMock.Verify(b => b.SearchLearningProvidersAsync(
                    It.Is<SearchRequest>(r => r.Take == 36),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldUsePointInTimeArgumentIfSpecified(DateTime pointInTime)
        {
            var context = BuildResolveFieldContext(pointInTime: pointInTime);

            await _resolver.ResolveAsync(context);

            _entityRepositoryMock.Verify(er => er.LoadLearningProvidersAsync(
                    It.Is<LoadLearningProvidersRequest>(r =>
                        r.PointInTime == pointInTime),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldUseBuiltEntityReferencesToLoadData(AggregateEntityReference[] entityReferences)
        {
            _registryProviderMock.Setup(r =>
                    r.SearchLearningProvidersAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResultSet
                {
                    Results = entityReferences.Select(r =>
                        new SearchResult
                        {
                            Entities = r.AdapterRecordReferences,
                        }).ToArray(),
                });
            var context = BuildResolveFieldContext();

            await _resolver.ResolveAsync(context);

            _entityRepositoryMock.Verify(r => r.LoadLearningProvidersAsync(
                    It.Is<LoadLearningProvidersRequest>(req => req.EntityReferences.Length == entityReferences.Length),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldRequestFieldsFromGraphQuery(AggregateEntityReference[] entityReferences)
        {
            _registryProviderMock.Setup(r =>
                    r.SearchLearningProvidersAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResultSet
                {
                    Results = entityReferences.Select(r =>
                        new SearchResult
                        {
                            Entities = r.AdapterRecordReferences,
                        }).ToArray(),
                });
            var fields = new[] {"name", "urn", "ukprn"};
            var context = BuildResolveFieldContext(fields: fields);

            await _resolver.ResolveAsync(context);

            _entityRepositoryMock.Verify(r => r.LoadLearningProvidersAsync(
                    It.Is<LoadLearningProvidersRequest>(req =>
                        req.Fields != null &&
                        req.Fields.Length == 3 &&
                        req.Fields.Contains("urn") &&
                        req.Fields.Contains("ukprn") &&
                        req.Fields.Contains("name")),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldAlwaysUseUrnAndUkprnInFieldsWhenGettingReferencedEntities(
            AggregateEntityReference[] entityReferences)
        {
            _registryProviderMock.Setup(r =>
                    r.SearchLearningProvidersAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResultSet
                {
                    Results = entityReferences.Select(r =>
                        new SearchResult
                        {
                            Entities = r.AdapterRecordReferences,
                        }).ToArray(),
                });
            var fields = new[] {"name", "postcode"};
            var context = BuildResolveFieldContext(fields: fields);

            await _resolver.ResolveAsync(context);

            _entityRepositoryMock.Verify(r => r.LoadLearningProvidersAsync(
                    It.Is<LoadLearningProvidersRequest>(req =>
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

            Assert.IsNotNull(actual.Results);
            Assert.AreEqual(entities.Length, actual.Results.Length);
            for (var i = 0; i < entities.Length; i++)
            {
                Assert.AreSame(entities[i], actual.Results[i],
                    $"Expected {i} to be {entities[i]} but was {actual.Results[i]}");
            }
        }

        [Test, AutoData]
        public async Task ThenItShouldReturnPaginationDetailsFromSearchResults(int skipped, int taken, int totalNumberOfRecords)
        {
            _registryProviderMock.Setup(r =>
                    r.SearchLearningProvidersAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResultSet
                {
                    Results = new[]
                    {
                        new SearchResult
                        {
                            Entities = new EntityReference[0],
                        },
                    },
                    Skipped = skipped,
                    Taken = taken,
                    TotalNumberOfRecords = totalNumberOfRecords,
                });
            var context = BuildResolveFieldContext();

            var actual = await _resolver.ResolveAsync(context);

            Assert.IsNotNull(actual.Pagination);
            Assert.AreEqual(skipped, actual.Pagination.Skipped);
            Assert.AreEqual(taken, actual.Pagination.Taken);
            Assert.AreEqual(totalNumberOfRecords, actual.Pagination.TotalNumberOfRecords);
        }


        private ResolveFieldContext<object> BuildResolveFieldContext(string name = null, string[] fields = null, int? skip = null, int? take = null, DateTime? pointInTime = null)
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
                {"isAnd", true},
                {"groups", groups}
            };

            var arguments = new Dictionary<string, object>
            {
                {"criteria", criteria},
            };

            if (skip.HasValue)
            {
                arguments.Add("skip", skip.Value);
            }

            if (take.HasValue)
            {
                arguments.Add("take", take.Value);
            }
            
            if (pointInTime.HasValue)
            {
                arguments.Add("pointInTime", pointInTime);
            }

            var context = TestHelper.BuildResolveFieldContext<object>(arguments: arguments, fields: new[] {"results", "_pagination"});
            
            var resultsField = (Field)context.FieldAst.SelectionSet.Selections
                .Single(x => ((Field) x).Name.Equals("results", StringComparison.InvariantCultureIgnoreCase));
            if (resultsField.SelectionSet == null)
            {
                resultsField.SelectionSet = new SelectionSet();
            }

            if (fields != null)
            {
                foreach (var field in fields)
                {
                    resultsField.SelectionSet.Add(new Field(null, new NameNode(field)));
                }
            }

            return context;
        }
    }
}