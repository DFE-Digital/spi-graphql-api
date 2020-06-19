using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.Models;
using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Context;
using Dfe.Spi.GraphQlApi.Domain.Registry;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using Dfe.Spi.Models.Entities;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface ICensusResolver : IResolver<Models.Entities.Census>
    {
    }

    public class CensusResolver : ICensusResolver
    {
        private static readonly ReadOnlyDictionary<string, DataOperator> DataOperators;

        private readonly IEntityRepository _entityRepository;
        private readonly IRegistryProvider _registryProvider;
        private readonly IGraphExecutionContextManager _executionContextManager;
        private readonly ILoggerWrapper _logger;

        static CensusResolver()
        {
            var dataOperators = new Dictionary<string, DataOperator>(); 
            var dataOperatorValues = Enum.GetValues(typeof(DataOperator));
            foreach (DataOperator value in dataOperatorValues)
            {
                var name = Enum.GetName(typeof(DataOperator), value);
                dataOperators.Add(name.ToUpper(), value);
            }
            DataOperators = new ReadOnlyDictionary<string, DataOperator>(dataOperators);
        }
        public CensusResolver(
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

        public async Task<Models.Entities.Census> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            if (context.Source is Models.Entities.LearningProvider)
            {
                return await ResolveLearningProviderAsync(context);
            }

            if (context.Source is Models.Entities.ManagementGroup)
            {
                return await ResolveManagementGroupAsync(context);
            }
            
            throw new NotImplementedException($"Expected to resolve for either LearningProvider or ManagementGroup, but used for {context.Source.GetType()}");
        }


        private async Task<Models.Entities.Census> ResolveLearningProviderAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            var entityId = BuildLearningProviderEntityId(context);
                
            try
            {
                return await ResolveEntitiesAsync(new[] {entityId}, context);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error resolving census for {entityId}: {ex.Message}", ex);
                throw;
            }
        }
        
        private async Task<Models.Entities.Census> ResolveManagementGroupAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            var managementGroup = context.Source as Models.Entities.ManagementGroup;
            try
            {
                var entityIds = await GetLearningProviderEntityIdsForManagementGroup(context);
                return await ResolveEntitiesAsync(entityIds, context);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error resolving census for {managementGroup.Code}: {ex.Message}", ex);
                throw;
            }
        }
        private async Task<string[]> GetLearningProviderEntityIdsForManagementGroup<TContext>(ResolveFieldContext<TContext> context)
        {
            var managementGroup = context.Source as ManagementGroup;
            var links = await _registryProvider.GetLinksAsync("management-groups",
                SourceSystemNames.GetInformationAboutSchools, managementGroup.Code, context.CancellationToken);
            var giasUrns = links
                .Where(link => link.LinkType == "ManagementGroup")
                .Select(link => long.Parse(link.SourceSystemId))
                .ToArray();
            return giasUrns.Select(urn => BuildLearningProviderEntityId<TContext>(urn, context)).ToArray();
        }
        
        private async Task<Models.Entities.Census> ResolveEntitiesAsync<TContext>(string[] entityIds, ResolveFieldContext<TContext> context)
        {
            var aggregationRequest = DeserializeAggregationRequests(context);

            var request = new LoadCensusRequest
            {
                EntityReferences = new[]
                {
                    new AggregateEntityReference
                    {
                        AdapterRecordReferences = entityIds.Select(id=>
                            new EntityReference
                            {
                                SourceSystemId = id,
                                SourceSystemName = SourceSystemNames.IStore,
                            }).ToArray(),
                    },
                },
                AggregatesRequest = aggregationRequest,
                Live = _executionContextManager.GraphExecutionContext.QueryLive,
            };
            var censuses = await _entityRepository.LoadCensusAsync(request, context.CancellationToken);
            return censuses.SquashedEntityResults.FirstOrDefault()?.SquashedEntity;
        }
        private string BuildLearningProviderEntityId<TContext>(ResolveFieldContext<TContext> context)
        {
            var sourceLearningProvider = context.Source as Models.Entities.LearningProvider;
            return sourceLearningProvider?.Urn == null 
                ? null 
                : BuildLearningProviderEntityId(sourceLearningProvider.Urn.Value, context);
        }
        private string BuildLearningProviderEntityId<TContext>(long urn, ResolveFieldContext<TContext> context)
        {
            var year = context.Arguments["year"];
            var type = context.Arguments["type"];

            return $"{year}_{type}-{nameof(Models.Entities.LearningProvider)}-{urn}";
        }
        private AggregatesRequest DeserializeAggregationRequests<TContext>(
            ResolveFieldContext<TContext> context)
        {
            var aggregations = context.FieldAst.SelectionSet.Selections
                .Select(x => (Field) x)
                .SingleOrDefault(f => f.Name == "_aggregations");
            if (aggregations == null)
            {
                return null;
            }

            var definitionsArgument = aggregations.Arguments.SingleOrDefault(a => a.Name == "definitions");
            if (definitionsArgument == null)
            {
                return null;
            }

            var definitions = (List<object>) definitionsArgument.Value.Value;
            var request = new AggregatesRequest();
            
            foreach (Dictionary<string, object> definition in definitions)
            {
                var name = (string) definition["name"];
                var conditions = (List<object>) definition["conditions"];
                var dataFilters = new List<DataFilter>();

                foreach (Dictionary<string, object> condition in conditions)
                {
                    var conditionOperator = DataOperator.Equals;
                    if (condition.ContainsKey("operator"))
                    {
                        var specifiedOperator = ((string) condition["operator"]).Replace("_", "").ToUpper();
                        if (DataOperators.ContainsKey(specifiedOperator))
                        {
                            conditionOperator = DataOperators[specifiedOperator];
                        }
                    }
                    
                    dataFilters.Add(new DataFilter
                    {
                        Field = (string)condition["field"],
                        Operator = conditionOperator,
                        Value = (string)condition["value"],
                    });
                }
                
                request.AggregateQueries.Add(name, new AggregateQuery { DataFilters = dataFilters.ToArray() });
            }

            return request;
        }
    }
}