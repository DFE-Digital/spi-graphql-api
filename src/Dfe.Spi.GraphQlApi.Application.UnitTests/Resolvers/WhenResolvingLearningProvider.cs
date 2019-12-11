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
        private LearningProviderResolver _resolver;

        [SetUp]
        public void Arrange()
        {
            _searchProviderMock = new Mock<ISearchProvider>();
            _searchProviderMock.Setup(p =>
                    p.SearchLearningProvidersAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResultSet<LearningProviderReference>
                {
                    Documents = new[]
                    {
                        new LearningProviderReference(),
                    }
                });

            _entityRepositoryMock = new Mock<IEntityRepository>();
            _entityRepositoryMock.Setup(r =>
                    r.LoadLearningProvidersAsync(It.IsAny<LoadLearningProvidersRequest>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(new EntityCollection<LearningProvider>
                {
                    Entities = new LearningProvider[0],
                });

            _resolver = new LearningProviderResolver(
                _searchProviderMock.Object,
                _entityRepositoryMock.Object);
        }

        [Test]
        public async Task ThenItShouldReturnArrayOfLearningProviders()
        {
            var context = BuildResolveFieldContext();

            var actual = await _resolver.ResolveAsync(context);

            Assert.IsNotNull(actual);
        }

        [Test, AutoData]
        public async Task ThenItShouldUseNameArgToSearchForLearningProviderReferences(string name)
        {
            var context = BuildResolveFieldContext(name);

            await _resolver.ResolveAsync(context);

            _searchProviderMock.Verify(
                p => p.SearchLearningProvidersAsync(It.Is<SearchRequest>(r => IsSearchRequestWithNameFilter(r, name)),
                    context.CancellationToken),
                Times.Once);
        }

        [Test, AutoData]
        public async Task ThenItShouldUseSearchReferencesToLoadEntities(
            SearchResultSet<LearningProviderReference> searchResults)
        {
            _searchProviderMock.Setup(p =>
                    p.SearchLearningProvidersAsync(It.IsAny<SearchRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(searchResults);
            var context = BuildResolveFieldContext();

            await _resolver.ResolveAsync(context);

            _entityRepositoryMock.Verify(r =>
                r.LoadLearningProvidersAsync(
                    It.Is<LoadLearningProvidersRequest>(req => IsLoadRequestForSearchResults(req, searchResults)),
                    context.CancellationToken));
        }

        [Test, NonRecursiveAutoData]
        public async Task ThenItShouldReturnEntitiesFromRepo(EntityCollection<LearningProvider> entities)
        {
            _entityRepositoryMock.Setup(r =>
                    r.LoadLearningProvidersAsync(It.IsAny<LoadLearningProvidersRequest>(),
                        It.IsAny<CancellationToken>()))
                .ReturnsAsync(entities);
            var context = BuildResolveFieldContext();

            var actual = await _resolver.ResolveAsync(context);

            Assert.AreSame(entities.Entities, actual);
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
                if (!request.EntityReferences.Any(r =>
                    r.SourceSystemName == document.SourceSystemName &&
                    r.SourceSystemId == document.SourceSystemId))
                {
                    return false;
                }
            }

            return true;
        }
    }
}