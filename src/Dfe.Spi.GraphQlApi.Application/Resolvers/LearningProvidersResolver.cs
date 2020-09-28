using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Application.GraphTypes;
using Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Context;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using GraphQL;
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
        private readonly IGraphExecutionContextManager _executionContextManager;
        private readonly ILoggerWrapper _logger;

        public LearningProvidersResolver(
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

        public async Task<LearningProvidersPagedModel> ResolveAsync<T>(ResolveFieldContext<T> context)
        {
            try
            {
                var resultSet = await SearchAsync(context, context.CancellationToken);
                var references = resultSet.Results.Select(d =>
                        new AggregateEntityReference {AdapterRecordReferences = d.Entities})
                    .ToArray();

                var fields = GetRequestedFields(context);
                var entities = await LoadAsync(references, context.Arguments, fields, context.CancellationToken);

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
            catch (InvalidRequestException ex)
            {
                _logger.Info($"Invalid request when resolving learning providers - {ex.ErrorIdentifier} - {ex.Message}", ex);
                context.Errors.AddRange(
                    ex.Details.Select(detailsMessage => new ExecutionError(detailsMessage)));
                return null;
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

        private async Task<LearningProvider[]> LoadAsync(
            AggregateEntityReference[] references, 
            Dictionary<string, object> arguments,
            string[] fields,
            CancellationToken cancellationToken)
        {
            if (references.Length == 0)
            {
                return new LearningProvider[0];
            }
            
            var request = new LoadLearningProvidersRequest
            {
                EntityReferences = references,
                Fields = fields,
                Live = _executionContextManager.GraphExecutionContext.QueryLive,
                PointInTime = arguments.SingleOrDefault(kvp => kvp.Key == "pointInTime").Value as DateTime?,
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
            var pointInTime = context.GetPointInTimeArgument();

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
                PointInTime = pointInTime,
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