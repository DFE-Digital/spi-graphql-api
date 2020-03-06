using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.Models;
using Dfe.Spi.Common.WellKnownIdentifiers;
using Dfe.Spi.GraphQlApi.Domain.Common;
using Dfe.Spi.GraphQlApi.Domain.Repository;
using GraphQL.Language.AST;
using GraphQL.Types;
using LearningProvider = Dfe.Spi.Models.Entities.LearningProvider;

namespace Dfe.Spi.GraphQlApi.Application.Resolvers
{
    public interface ICensusResolver : IResolver<Models.Entities.Census>
    {
    }

    public class CensusResolver : ICensusResolver
    {
        private static readonly ReadOnlyDictionary<string, DataOperator> DataOperators;

        private readonly IEntityRepository _entityRepository;
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
            ILoggerWrapper logger)
        {
            _entityRepository = entityRepository;
            _logger = logger;
        }

        public async Task<Models.Entities.Census> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            var aggregationRequest = DeserializeAggregationRequests(context);
            var entityId = BuildEntityId(context);

            var request = new LoadCensusRequest
            {
                EntityReferences = new[]
                {
                    new AggregateEntityReference
                    {
                        AdapterRecordReferences = new[]
                        {
                            new EntityReference
                            {
                                SourceSystemId = entityId,
                                SourceSystemName = SourceSystemNames.IStore,
                            },
                        }
                    },
                },
                AggregatesRequest = aggregationRequest,
            };
            var censuses = await _entityRepository.LoadCensusAsync(request, context.CancellationToken);
            return censuses.SquashedEntityResults.FirstOrDefault()?.SquashedEntity;
        }


        private string BuildEntityId<TContext>(ResolveFieldContext<TContext> context)
        {
            var sourceLearningProvider = context.Source as LearningProvider;
            
            var year = context.Arguments["year"];
            var type = context.Arguments["type"];

            return $"{year}_{type}-{nameof(LearningProvider)}-{sourceLearningProvider.Urn}";
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