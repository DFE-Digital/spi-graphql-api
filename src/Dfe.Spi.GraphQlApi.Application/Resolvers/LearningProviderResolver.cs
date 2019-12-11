using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.GraphQlApi.Domain.Repository;
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
        private readonly IEntityRepository _entityRepository;

        public LearningProviderResolver(
            ISearchProvider searchProvider,
            IEntityRepository entityRepository)
        {
            _searchProvider = searchProvider;
            _entityRepository = entityRepository;
        }

        public async Task<LearningProvider[]> ResolveAsync<T>(ResolveFieldContext<T> context)
        {
            var references = await SearchAsync(context, context.CancellationToken);

            var entities = await LoadAsync(references, context.CancellationToken);

            return entities;
        }

        private async Task<LearningProviderReference[]> SearchAsync<T>(ResolveFieldContext<T> context,
            CancellationToken cancellationToken)
        {
            var request = new SearchRequest
            {
                Filter = new[]
                {
                    new SearchFilter
                    {
                        Field = "Name",
                        Value = (string) context.Arguments["name"],
                    },
                }
            };

            var searchResults =
                await _searchProvider.SearchLearningProvidersAsync(request, cancellationToken);

            return searchResults.Documents;
        }

        private async Task<LearningProvider[]> LoadAsync(LearningProviderReference[] references,
            CancellationToken cancellationToken)
        {
            var request = new LoadLearningProvidersRequest
            {
                EntityReferences = references,
            };
            var loadResult = await _entityRepository.LoadLearningProvidersAsync(request, cancellationToken);

            return loadResult.Entities;
        }
    }
}