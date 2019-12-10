using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using Dfe.Spi.GraphQlApi.Domain.Search;
using GraphQL.Types;
using Moq;
using NUnit.Framework;

namespace Dfe.Spi.GraphQlApi.Application.UnitTests.Resolvers
{
    public class WhenResolvingLearningProvider
    {
        private Mock<ISearchProvider> _searchProviderMock;
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

            _resolver = new LearningProviderResolver(_searchProviderMock.Object);
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

        
        
        
        private ResolveFieldContext<object> BuildResolveFieldContext(string name = null)
        {
            return TestHelper.BuildResolveFieldContext<object>(arguments: new Dictionary<string, object>
            {
                {"name", name ?? Guid.NewGuid().ToString()},
            });
        }
        private bool IsSearchRequestWithNameFilter(SearchRequest searchRequest, string name)
        {
            return searchRequest.Filters.Any(f => f.Field == "Name" && f.Value == name);
        }
    }
}