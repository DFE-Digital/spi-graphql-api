using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.GraphQlApi.Domain.Search;
using Dfe.Spi.Models.Entities;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface ILearningProviderResolver : IResolver<LearningProvider>
    {
    }

    public class LearningProviderResolver : ILearningProviderResolver
    {
        private readonly IEntityRepository _entityRepository;
        private readonly ILoggerWrapper _logger;
        private readonly IEntityReferenceBuilder _entityReferenceBuilder;

        internal LearningProviderResolver(
            IEntityRepository entityRepository,
            ILoggerWrapper logger,
            IEntityReferenceBuilder entityReferenceBuilder)
        {
            _entityRepository = entityRepository;
            _logger = logger;
            _entityReferenceBuilder = entityReferenceBuilder;
        }

        public LearningProviderResolver(
            ISearchProvider searchProvider,
            IEntityRepository entityRepository,
            IRegistryProvider registryProvider,
            ILoggerWrapper logger)
        {
            _entityRepository = entityRepository;
            _logger = logger;

            Task<EntityReference[]> GetSynonyms(string sourceSystem, string sourceId,
                CancellationToken cancellationToken) =>
                registryProvider.GetSynonymsAsync("learning-providers", sourceSystem, sourceId, cancellationToken);

            _entityReferenceBuilder = new EntityReferenceBuilder<LearningProviderReference>(
                searchProvider.SearchLearningProvidersAsync,
                GetSynonyms);
        }

        public async Task<LearningProvider> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            try
            {
                var reference = await SearchAsync(context.Arguments, context.CancellationToken);
                if (reference == null)
                {
                    return null;
                }
                _logger.Info($"Found reference {reference}");

                var fields = GetRequestedFields(context);
                var entity = await LoadAsync(reference, fields, context.CancellationToken);

                return entity;
            }
            catch (ResolverException ex)
            {
                _logger.Info($"Request issue resolving learning provider", ex);
                context.Errors.Add(new ExecutionError(ex.Message));
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error resolving learning provider", ex);
                throw;
            }
        }

        private async Task<AggregateEntityReference> SearchAsync(Dictionary<string, object> arguments,
            CancellationToken cancellationToken)
        {
            var filters = arguments
                .Select(kvp => new SearchFilter
                {
                    Field = kvp.Key,
                    Value = kvp.Value?.ToString(),
                }).ToList();
            if (filters.Count != 1)
            {
                throw new ResolverException("Must provide one argument");
            }

            var references = await _entityReferenceBuilder.GetEntityReferences(new SearchRequest
            {
                Filter = filters.ToArray(),
            }, cancellationToken);
            return references.FirstOrDefault();
        }

        private async Task<LearningProvider> LoadAsync(AggregateEntityReference reference, string[] fields,
            CancellationToken cancellationToken)
        {
            var request = new LoadLearningProvidersRequest
            {
                EntityReferences = new[] {reference},
                Fields = fields,
            };
            var loadResult = await _entityRepository.LoadLearningProvidersAsync(request, cancellationToken);

            return loadResult.SquashedEntityResults.Select(x => x.SquashedEntity).SingleOrDefault();
        }

        private string[] GetRequestedFields<T>(ResolveFieldContext<T> context)
        {
            var selections = context.FieldAst.SelectionSet.Selections.Select(x => ((Field) x).Name);
            
            // Will need identifiers for resolving sub objects (such as management group), so request them from backend
            selections = selections.Concat(new[] {"urn", "ukprn"}).Distinct();
            
            return selections.ToArray();
        }
    }
}