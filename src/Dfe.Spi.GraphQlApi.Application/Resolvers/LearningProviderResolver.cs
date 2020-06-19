using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Context;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
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
        private readonly IRegistryProvider _registryProvider;
        private readonly IGraphExecutionContextManager _executionContextManager;

        public LearningProviderResolver(
            IEntityRepository entityRepository,
            IRegistryProvider registryProvider,
            IGraphExecutionContextManager executionContextManager,
            ILoggerWrapper logger)
        {
            _entityRepository = entityRepository;
            _registryProvider = registryProvider;
            _executionContextManager = executionContextManager;
            _logger = logger;
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
            catch (InvalidRequestException ex)
            {
                _logger.Info($"Invalid request when resolving learning provider - {ex.ErrorIdentifier} - {ex.Message}", ex);
                context.Errors.AddRange(
                    ex.Details.Select(detailsMessage => new ExecutionError(detailsMessage)));
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

            var searchRequest = new SearchRequest
            {
                Groups = new[]
                {
                    new SearchGroup
                    {
                        Filter = filters.ToArray(),
                        CombinationOperator = "and",
                    },
                },
                CombinationOperator = "and",
                Skip = 0,
                Take = 25,
            };
            var searchResults = await _registryProvider.SearchLearningProvidersAsync(searchRequest, cancellationToken);
            int CalculateOrder(SearchResult searchResult)
            {
                var status = searchResult.IndexedData?
                    .SingleOrDefault(kvp => kvp.Key.Equals("Status", StringComparison.InvariantCultureIgnoreCase))
                    .Value;
                if (status != null && status.Equals("Open", StringComparison.InvariantCultureIgnoreCase))
                {
                    return 1;
                }
                return int.MaxValue;
            };
            var result = searchResults.Results
                .Select(searchResult =>
                    new
                    {
                        Result = searchResult,
                        Order = CalculateOrder(searchResult),
                    })
                .OrderBy(orderedResult => orderedResult.Order)
                .FirstOrDefault()?.Result;
            return result == null 
                ? null 
                : new AggregateEntityReference {AdapterRecordReferences = result.Entities};
        }

        private async Task<LearningProvider> LoadAsync(AggregateEntityReference reference, string[] fields,
            CancellationToken cancellationToken)
        {
            var request = new LoadLearningProvidersRequest
            {
                EntityReferences = new[] {reference},
                Fields = fields,
                Live = _executionContextManager.GraphExecutionContext.QueryLive,
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