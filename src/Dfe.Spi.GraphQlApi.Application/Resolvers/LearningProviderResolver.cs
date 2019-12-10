using System.Linq;
using System.Threading.Tasks;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Dfe.Spi.Models;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface ILearningProviderResolver : IResolver<LearningProvider>
    {
    }

    public class LearningProviderResolver : ILearningProviderResolver
    {
        private readonly ISearchProvider _searchProvider;

        public LearningProviderResolver(ISearchProvider searchProvider)
        {
            _searchProvider = searchProvider;
        }

        public async Task<LearningProvider[]> ResolveAsync<T>(ResolveFieldContext<T> context)
        {
            var searchResults = await _searchProvider.SearchLearningProvidersAsync(new SearchRequest
            {
                Filters = new[]
                {
                    new SearchFilter
                    {
                        Field = "Name",
                        Value = (string) context.Arguments["name"],
                    },
                }
            }, context.CancellationToken);

            return searchResults.Documents.Select(d => new LearningProvider
            {
                Name = d.Name,
            }).ToArray();
        }
    }
}