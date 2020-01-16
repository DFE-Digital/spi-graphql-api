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
        private readonly IEntityReferenceBuilder<LearningProviderReference> _entityReferenceBuilder;

        internal LearningProviderResolver(
            ISearchProvider searchProvider,
            IEntityRepository entityRepository,
            IEntityReferenceBuilder<LearningProviderReference> entityReferenceBuilder)
        {
            _searchProvider = searchProvider;
            _entityRepository = entityRepository;
            _entityReferenceBuilder = entityReferenceBuilder;
        }
        public LearningProviderResolver(
            ISearchProvider searchProvider,
            IEntityRepository entityRepository)
        {
            _searchProvider = searchProvider;
            _entityRepository = entityRepository;
            
            _entityReferenceBuilder = new EntityReferenceBuilder<LearningProviderReference>(
                _searchProvider.SearchLearningProvidersAsync);
        }

        public async Task<LearningProvider[]> ResolveAsync<T>(ResolveFieldContext<T> context)
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
            var references = await _entityReferenceBuilder.GetEntityReferences(request, context.CancellationToken);

            var entities = await LoadAsync(references, context.CancellationToken);

            return entities;
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