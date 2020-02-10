using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Dfe.Spi.Models;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface ILearningProvidersResolver : IResolver<LearningProvider[]>
    {
    }

    public class LearningProvidersResolver : ILearningProvidersResolver
    {
        private readonly IEntityRepository _entityRepository;
        private readonly ILoggerWrapper _logger;
        private readonly IEntityReferenceBuilder _entityReferenceBuilder;

        internal LearningProvidersResolver(
            IEntityRepository entityRepository,
            ILoggerWrapper logger,
            IEntityReferenceBuilder entityReferenceBuilder)
        {
            _entityRepository = entityRepository;
            _logger = logger;
            _entityReferenceBuilder = entityReferenceBuilder;
        }
        public LearningProvidersResolver(
            ISearchProvider searchProvider,
            IEntityRepository entityRepository,
            IRegistryProvider registryProvider,
            ILoggerWrapper logger)
        {
            _entityRepository = entityRepository;
            _logger = logger;

            Task<EntityReference[]> GetSynonyms(string sourceSystem, string sourceId, CancellationToken cancellationToken) => 
                registryProvider.GetSynonymsAsync("learning-providers", sourceSystem, sourceId, cancellationToken);

            _entityReferenceBuilder = new EntityReferenceBuilder<LearningProviderReference>(
                searchProvider.SearchLearningProvidersAsync,
                GetSynonyms);
        }

        public async Task<LearningProvider[]> ResolveAsync<T>(ResolveFieldContext<T> context)
        {
            try
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

                var fields = GetRequestedFields(context);
                var entities = await LoadAsync(references, fields, context.CancellationToken);

                return entities;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error resolving learning providers", ex);
                throw;
            }
        }

        private async Task<LearningProvider[]> LoadAsync(AggregateEntityReference[] references, string[] fields,
            CancellationToken cancellationToken)
        {
            var request = new LoadLearningProvidersRequest
            {
                EntityReferences = references,
                Fields = fields,
            };
            var loadResult = await _entityRepository.LoadLearningProvidersAsync(request, cancellationToken);

            return loadResult.SquashedEntityResults.Select(x => x.SquashedEntity).ToArray();
        }

        private string[] GetRequestedFields<T>(ResolveFieldContext<T> context)
        {
            var selections = context.FieldAst.SelectionSet.Selections.Select(x=>((Field)x).Name);
            return selections.ToArray();
        }
    }
}