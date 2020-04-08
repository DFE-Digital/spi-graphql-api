using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Application.GraphTypes;
using Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using GraphQL.Language.AST;
using GraphQL.Types;
using LearningProvider = Dfe.Spi.Models.Entities.LearningProvider;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface ILearningProvidersResolver : IResolver<LearningProvidersPagedModel>
    {
    }

    public class LearningProvidersResolver : ILearningProvidersResolver
    {
        private readonly IEntityRepository _entityRepository;
        private readonly IRegistryProvider _registryProvider;
        private readonly ILoggerWrapper _logger;

        public LearningProvidersResolver(
            IEntityRepository entityRepository,
            IRegistryProvider registryProvider,
            ILoggerWrapper logger)
        {
            _entityRepository = entityRepository;
            _registryProvider = registryProvider;
            _logger = logger;
        }

        public async Task<LearningProvidersPagedModel> ResolveAsync<T>(ResolveFieldContext<T> context)
        {
            try
            {
                var resultSet = await SearchAsync(context, context.CancellationToken);
                var references = resultSet.Results.Select(d =>
                        new AggregateEntityReference {AdapterRecordReferences = d.Entities})
                    .ToArray();

                var fields = GetRequestedFields(context);
                var entities = await LoadAsync(references, fields, context.CancellationToken);

                return new LearningProvidersPagedModel
                {
                    Results = entities,
                    Pagination = new PaginationDetailsModel
                    {
                        Skipped = resultSet.Skipped,
                        Taken = resultSet.Taken,
                        TotalNumberOfRecords = resultSet.TotalNumberOfRecords,
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.Error($"Error resolving learning providers", ex);
                throw;
            }
        }

        private async Task<SearchResultSet> SearchAsync<T>(ResolveFieldContext<T> context, CancellationToken cancellationToken)
        {
            var searchRequest = GetSearchRequest(context);
            var searchResults = await _registryProvider.SearchLearningProvidersAsync(searchRequest, cancellationToken);
            return searchResults;
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

        private SearchRequest GetSearchRequest<T>(ResolveFieldContext<T> context)
        {
            var criteria = (ComplexQueryModel) context.GetArgument(typeof(ComplexQueryModel), "criteria");
            var skip = context.HasArgument("skip")
                ? (int) context.Arguments["skip"]
                : 0;
            var take = context.HasArgument("take")
                ? (int) context.Arguments["take"]
                : 50;

            var searchGroups = new List<SearchGroup>();
            foreach (var @group in criteria.Groups)
            {
                var filters = @group.Conditions.Select(c => new SearchFilter
                {
                    Field = c.Field,
                    Operator = c.Operator,
                    Value = c.Value,
                }).ToArray();
                
                searchGroups.Add(new SearchGroup
                {
                    Filter = filters,
                    CombinationOperator = group.IsOr ? "or" : "and",
                });
            }

            return new SearchRequest
            {
                Groups = searchGroups.ToArray(),
                CombinationOperator = criteria.IsOr ? "or" : "and",
                Skip = skip,
                Take = take,
            };
        }

        private string[] GetRequestedFields<T>(ResolveFieldContext<T> context)
        {
            var defaultFields = new[] {"urn", "ukprn"};
            
            var resultsField = (Field)context.FieldAst.SelectionSet.Selections
                .SingleOrDefault(x=>((Field) x).Name.Equals("results", StringComparison.InvariantCultureIgnoreCase));
            if (resultsField == null)
            {
                return defaultFields;
            }
            
            var selections = resultsField.SelectionSet.Selections.Select(x => ((Field) x).Name);
            
            // Will need identifiers for resolving sub objects (such as management group), so request them from backend
            return selections
                .Concat(defaultFields)
                .Distinct()
                .ToArray();
        }
    }
}