using System.Threading;
using System.Threading.Tasks;

namespace Dfe.Spi.GraphQlApi.Domain.Search
{
    public interface ISearchProvider
    {
        Task<SearchResultSet<LearningProviderReference>> SearchLearningProvidersAsync(SearchRequest request, CancellationToken cancellationToken);
    }
}