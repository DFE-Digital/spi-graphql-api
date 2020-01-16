using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.Common.UnitTesting.Fixtures;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Dfe.Spi.Models;
using GraphQL.Types;
using Moq;
using NUnit.Framework;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests.Resolvers
{
    public class WhenResolvingLearningProvider
    {
        private Mock<ISearchProvider> _searchProviderMock;
        private Mock<IEntityRepository> _entityRepositoryMock;
        private Mock<IEntityReferenceBuilder<LearningProviderReference>> _entityReferenceBuilderMock;
        private LearningProviderResolver _resolver;

        [SetUp]
        public void Arrange()
        {
            _searchProviderMock = new Mock<ISearchProvider>();

            _entityRepositoryMock = new Mock<IEntityRepository>();
            _entityRepositoryMock.Setup(r =>
                    r.LoadLearningProvidersAsync(It.IsAny<LoadLearningProvidersRequest>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityCollection<LearningProvider>
                {
                    SquashedEntityResults = new SquashedEntityResult<LearningProvider>[0],
                });
            
            _entityReferenceBuilderMock = new Mock<IEntityReferenceBuilder<LearningProviderReference>>();
            _entityReferenceBuilderMock.Setup(b =>
                    b.GetEntityReferences(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new AggregateEntityReference<LearningProviderReference>[0]);

            _resolver = new LearningProviderResolver(
                _searchProviderMock.Object,
                _entityRepositoryMock.Object,
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
        public async Task ThenItShouldUseBuiltEntityReferencesToLoadData(AggregateEntityReference<LearningProviderReference>[] entityReferences)
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


        private ResolveFieldContext<object> BuildResolveFieldContext(string name = null)
        {
            return TestHelper.BuildResolveFieldContext<object>(arguments: new Dictionary<string, object>
            {
                {"name", name ?? Guid.NewGuid().ToString()},
            });
        }

        private bool IsSearchRequestWithNameFilter(SearchRequest searchRequest, string name)
        {
            return searchRequest.Filter.Any(f => f.Field == "Name" && f.Value == name);
        }

        private bool IsLoadRequestForSearchResults(LoadLearningProvidersRequest request,
            SearchResultSet<LearningProviderReference> searchResults)
        {
            if (request == null
                || request.EntityReferences == null
                || request.EntityReferences.Length != searchResults.Documents.Length)
            {
                return false;
            }

            foreach (var document in searchResults.Documents)
            {
                if (!request.EntityReferences.Any(er =>
                    er.AdapterRecordReferences.Any(arr =>
                        arr.SourceSystemName == document.SourceSystemName &&
                        arr.SourceSystemId == document.SourceSystemId)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}