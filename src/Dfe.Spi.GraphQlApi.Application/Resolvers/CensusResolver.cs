using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Dfe.Spi.Common.Logging.Definitions;
using Dfe.Spi.Common.Models;
using Dfe.Spi.GraphQlApi.Application.GraphTypes;
using Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs;
using Dfe.Spi.Models.Entities;
using GraphQL;
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
        public CensusResolver(ILoggerWrapper logger)
        {
            _logger = logger;
        }

        public Task<Models.Entities.Census> ResolveAsync<TContext>(ResolveFieldContext<TContext> context)
        {
            var aggregationRequests = DeserializeAggregationRequests(context);

            return Task.FromResult(new Models.Entities.Census
            {
                Name = $"{context.Arguments["year"]} {context.Arguments["type"]}",
                _Aggregations = aggregationRequests.Select(req=>
                    new Models.Aggregation
                    {
                        Name = req.Name,
                        Value = 12345.3234m,
                    }).ToArray(),
            });
        }


        private AggregationRequestModel[] DeserializeAggregationRequests<TContext>(
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
            var aggregationRequests = new List<AggregationRequestModel>();
            
            foreach (Dictionary<string, object> definition in definitions)
            {
                var name = (string) definition["name"];
                var conditions = (List<object>) definition["conditions"];
                var aggregationRequestConditions = new List<AggregationRequestConditionModel>();

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
                    
                    aggregationRequestConditions.Add(new AggregationRequestConditionModel
                    {
                        Field = (string)condition["field"],
                        Operator = conditionOperator,
                        Value = (string)condition["value"],
                    });
                }
                
                aggregationRequests.Add(new AggregationRequestModel
                {
                    Name = name,
                    Conditions = aggregationRequestConditions.ToArray(),
                });
            }

            return aggregationRequests.ToArray();
        }
    }
}