using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.GraphQlApi.Application.GraphTypes;
using Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using ManagementGroup = Dfe.Spi.Models.Entities.ManagementGroup;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface IManagementGroupsResolver : IResolver<ManagementGroupsPagedModel>
    {
    }
    public class ManagementGroupsResolver : IManagementGroupsResolver
    {
        private readonly IEntityRepository _entityRepository;
        private readonly IRegistryProvider _registryProvider;
        private readonly ILoggerWrapper _logger;

        public ManagementGroupsResolver(
            IEntityRepository entityRepository,
            IRegistryProvider registryProvider,
            ILoggerWrapper logger)
        {
            _entityRepository = entityRepository;
            _registryProvider = registryProvider;
            _logger = logger;
        }
        
        public async Task<ManagementGroupsPagedModel> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            try
            {
                var resultSet = await SearchAsync(context, context.CancellationToken);
                var references = resultSet.Results.Select(d =>
                        new AggregateEntityReference {AdapterRecordReferences = d.Entities})
                    .ToArray();

                var fields = GetRequestedFields(context);
                var entities = await LoadAsync(references, fields, context.CancellationToken);

                return new ManagementGroupsPagedModel
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
                _logger.Info($"Invalid request when resolving management groups - {ex.Message}", ex);
                context.Errors.AddRange(
                    ex.Details.Select(detailsMessage => new ExecutionError(detailsMessage)));
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error resolving management groups", ex);
                throw;
            }
        }

        private async Task<SearchResultSet> SearchAsync<T>(ResolveFieldContext<T> context, CancellationToken cancellationToken)
        {
            var searchRequest = GetSearchRequest(context);
            var searchResults = await _registryProvider.SearchManagementGroupsAsync(searchRequest, cancellationToken);
            return searchResults;
        }

        private async Task<ManagementGroup[]> LoadAsync(AggregateEntityReference[] references, string[] fields,
            CancellationToken cancellationToken)
        {
            var request = new LoadManagementGroupsRequest
            {
                EntityReferences = references,
                Fields = fields,
            };
            var loadResult = await _entityRepository.LoadManagementGroupsAsync(request, cancellationToken);

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
                    Field = MapSearchField(c.Field),
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

        private string MapSearchField(string searchValue)
        {
            switch (searchValue.ToLower())
            {
                case "type":
                    return "managementGroupType";
                case "code":
                    return "managementGroupId";
            }

            return searchValue;
        }

        private string[] GetRequestedFields<T>(ResolveFieldContext<T> context)
        {
            var selections = context.FieldAst.SelectionSet.Selections.Select(x => ((Field) x).Name);
            
            // Will need identifiers for resolving sub objects (such as census), so request them from backend
            selections = selections.Concat(new[] {"code"}).Distinct();

            return selections.ToArray();
        }
    }
}