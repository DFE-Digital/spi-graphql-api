using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Configuration;
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
            var searchResults = await SearchAsync(context, context.CancellationToken);

            var references = await GetSynonymousIdentifiersAsync(searchResults, context.CancellationToken);

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

        private async Task<AggregateEntityReference<LearningProviderReference>[]> GetSynonymousIdentifiersAsync(
            LearningProviderReference[] references,
            CancellationToken cancellationToken)
        {
            // TODO: Go to registry to expand
            return references.Select(r =>
                new AggregateEntityReference<LearningProviderReference>
                {
                    AdapterRecordReferences = new[] {r},
                }).ToArray();
        }

        private async Task<LearningProvider[]> LoadAsync(AggregateEntityReference<LearningProviderReference>[] references,
            CancellationToken cancellationToken)
        {
            var request = new LoadLearningProvidersRequest
            {
                EntityReferences = references,
            };
            var loadResult = await _entityRepository.LoadLearningProvidersAsync(request, cancellationToken);

            return loadResult.SquashedEntityResults.Select(x => x.SquashedEntity).ToArray();
        }
    }
}