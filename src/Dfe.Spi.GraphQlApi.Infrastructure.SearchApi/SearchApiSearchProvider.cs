using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.GraphQlApi.Domain.Search;

namespace Dfe.Spi.GraphQlApi.Infrastructure.SearchApi
{
    public class SearchApiSearchProvider : ISearchProvider
    {
        public async Task<SearchResultSet<LearningProviderReference>> SearchLearningProvidersAsync(SearchRequest request, CancellationToken cancellationToken)
        {
            return new SearchResultSet<LearningProviderReference>
            {
                Documents = new []
                {
                    new LearningProviderReference
                    {
                        Name = "Provider One",
                        SourceSystemName = "Static",
                        SourceSystemId = "1",
                    }, 
                    new LearningProviderReference
                    {
                        Name = "Provider Two",
                        SourceSystemName = "Static",
                        SourceSystemId = "2",
                    }, 
                },
            };
        }
    }
}