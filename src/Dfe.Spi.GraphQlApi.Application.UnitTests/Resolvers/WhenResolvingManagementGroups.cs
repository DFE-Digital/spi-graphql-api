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
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.Models.Entities;
using GraphQL.Types;
using Moq;
using NUnit.Framework;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests.Resolvers
{
    public class WhenResolvingManagementGroups
    {
        private Mock<IEntityRepository> _entityRepositoryMock;
        private Mock<IRegistryProvider> _registryProviderMock;
        private Mock<ILoggerWrapper> _loggerMock;
        private ManagementGroupsResolver _resolver;

        [SetUp]
        public void Arrange()
        {
            _entityRepositoryMock = new Mock<IEntityRepository>();
            _entityRepositoryMock.Setup(r =>
                    r.LoadManagementGroupsAsync(It.IsAny<LoadManagementGroupsRequest>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityCollection<ManagementGroup>
                {
                    SquashedEntityResults = new SquashedEntityResult<ManagementGroup>[0],
                });

            _registryProviderMock = new Mock<IRegistryProvider>();
            _registryProviderMock.Setup(r =>
                    r.SearchManagementGroupsAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
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

            _loggerMock = new Mock<ILoggerWrapper>();

            _resolver = new ManagementGroupsResolver(
                _entityRepositoryMock.Object,
                _registryProviderMock.Object,
                _loggerMock.Object);
        }

        [Test]
        public async Task ThenItShouldReturnArrayOfManagementGroups()
        {
            var context = BuildResolveFieldContext();

            var actual = await _resolver.ResolveAsync(context);

            Assert.IsNotNull(actual);
        }

        [Test, AutoData]
        public async Task ThenItShouldSearchWithProvidedCriteria(string identifier)
        {
            var context = BuildResolveFieldContext(identifier);

            await _resolver.ResolveAsync(context);

            _registryProviderMock.Verify(b => b.SearchManagementGroupsAsync(
                    It.Is<SearchRequest>(r =>
                        r.Groups != null &&
                        r.Groups.Length == 1 &&
                        r.Groups[0].Filter != null &&
                        r.Groups[0].Filter.Length == 1 &&
                        r.Groups[0].Filter[0].Field == "identifier" &&
                        r.Groups[0].Filter[0].Value == identifier),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldTranslateTypeArgToManagementGroupTypeField(string type)
        {
            var context = BuildResolveFieldContext(type: type);

            await _resolver.ResolveAsync(context);

            _registryProviderMock.Verify(b => b.SearchManagementGroupsAsync(
                    It.Is<SearchRequest>(r =>
                        r.Groups != null &&
                        r.Groups.Length == 1 &&
                        r.Groups[0].Filter != null &&
                        r.Groups[0].Filter.Length == 1 &&
                        r.Groups[0].Filter[0].Field == "managementGroupType" &&
                        r.Groups[0].Filter[0].Value == type),
                    context.CancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldDefaultToSkipZeroIfNotSpecified()
        {
            var context = BuildResolveFieldContext();

            await _resolver.ResolveAsync(context);

            _registryProviderMock.Verify(b => b.SearchManagementGroupsAsync(
                    It.Is<SearchRequest>(r => r.Skip == 0),
                    context.CancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldUseSkipArgumentIfSpecified()
        {
            var context = BuildResolveFieldContext(skip: 25);

            await _resolver.ResolveAsync(context);

            _registryProviderMock.Verify(b => b.SearchManagementGroupsAsync(
                    It.Is<SearchRequest>(r => r.Skip == 25),
                    context.CancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldDefaultToTake50IfNotSpecified()
        {
            var context = BuildResolveFieldContext();

            await _resolver.ResolveAsync(context);

            _registryProviderMock.Verify(b => b.SearchManagementGroupsAsync(
                    It.Is<SearchRequest>(r => r.Take == 50),
                    context.CancellationToken),
                Times.Once);
        }

        [Test]
        public async Task ThenItShouldUseTakeArgumentIfSpecified()
        {
            var context = BuildResolveFieldContext(take: 36);

            await _resolver.ResolveAsync(context);

            _registryProviderMock.Verify(b => b.SearchManagementGroupsAsync(
                    It.Is<SearchRequest>(r => r.Take == 36),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldUseBuiltEntityReferencesToLoadData(AggregateEntityReference[] entityReferences)
        {
            _registryProviderMock.Setup(r =>
                    r.SearchManagementGroupsAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
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

            _entityRepositoryMock.Verify(r => r.LoadManagementGroupsAsync(
                    It.Is<LoadManagementGroupsRequest>(req => req.EntityReferences.Length == entityReferences.Length),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldRequestFieldsFromGraphQuery(AggregateEntityReference[] entityReferences)
        {
            _registryProviderMock.Setup(r =>
                    r.SearchManagementGroupsAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResultSet
                {
                    Results = entityReferences.Select(r =>
                        new SearchResult
                        {
                            Entities = r.AdapterRecordReferences,
                        }).ToArray(),
                });
            var fields = new[] {"code", "identifier", "name"};
            var context = BuildResolveFieldContext(fields: fields);

            await _resolver.ResolveAsync(context);

            _entityRepositoryMock.Verify(r => r.LoadManagementGroupsAsync(
                    It.Is<LoadManagementGroupsRequest>(req =>
                        req.Fields != null &&
                        req.Fields.Length == 3 &&
                        req.Fields.Contains("code") &&
                        req.Fields.Contains("identifier") &&
                        req.Fields.Contains("name")),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldAlwaysUseCodeInFieldsWhenGettingReferencedEntities(
            AggregateEntityReference[] entityReferences)
        {
            _registryProviderMock.Setup(r =>
                    r.SearchManagementGroupsAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResultSet
                {
                    Results = entityReferences.Select(r =>
                        new SearchResult
                        {
                            Entities = r.AdapterRecordReferences,
                        }).ToArray(),
                });
            var fields = new[] {"identifier", "name"};
            var context = BuildResolveFieldContext(fields: fields);

            await _resolver.ResolveAsync(context);

            _entityRepositoryMock.Verify(r => r.LoadManagementGroupsAsync(
                    It.Is<LoadManagementGroupsRequest>(req =>
                        req.Fields != null &&
                        req.Fields.Length == 3 &&
                        req.Fields.Contains("code") &&
                        req.Fields.Contains("identifier") &&
                        req.Fields.Contains("name")),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, NonRecursiveAutoData]
        public async Task ThenItShouldReturnEntitiesFromRepo(ManagementGroup[] entities)
        {
            _entityRepositoryMock.Setup(r =>
                    r.LoadManagementGroupsAsync(It.IsAny<LoadManagementGroupsRequest>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityCollection<ManagementGroup>
                {
                    SquashedEntityResults = entities.Select(e =>
                        new SquashedEntityResult<ManagementGroup>
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


        private ResolveFieldContext<object> BuildResolveFieldContext(string identifier = null, string type = null, string[] fields = null, int? skip = null, int? take = null)
        {
            var conditions = new List<object>();
            if (identifier != null)
            {
                conditions.Add(new Dictionary<string, object>
                {
                    {"field", "identifier"},
                    {"operator", "equals"},
                    {"value", identifier},
                });
            }
            if (type != null)
            {
                conditions.Add(new Dictionary<string, object>
                {
                    {"field", "type"},
                    {"operator", "equals"},
                    {"value", type},
                });
            }
            if (conditions.Count == 0)
            {
                conditions.Add(new Dictionary<string, object>
                {
                    {"field", "identifier"},
                    {"operator", "equals"},
                    {"value", Guid.NewGuid().ToString()},
                });
            }
            
            var groups = new List<object>
            {
                new Dictionary<string, object>
                {
                    {"isAnd", true},
                    {"conditions", conditions},
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
            
            return TestHelper.BuildResolveFieldContext<object>(arguments: arguments, fields: fields);
        }
    }
}