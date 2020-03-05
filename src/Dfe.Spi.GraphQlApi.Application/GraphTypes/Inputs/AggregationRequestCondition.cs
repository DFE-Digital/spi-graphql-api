using Dfe.Spi.Common.Models;
using Dfe.Spi.GraphQlApi.Application.GraphTypes.Enums;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs
{
    public class AggregationRequestConditionModel
    {
        public string Field { get; set; }
        public DataOperator Operator { get; set; }
        public string Value { get; set; }
    }
    
    public class AggregationRequestCondition : InputObjectGraphType<AggregationRequestConditionModel>
    {
        public AggregationRequestCondition()
        {
            Field<NonNullGraphType<CensusAggregationFieldsEnum>>(
                name: "field",
                description: "Name of the field to query on",
                resolve: ctx => ctx.Source.Field);
            
            Field<OperatorEnum>(
                name: "operator",
                description: "Operator to use to compare",
                resolve: ctx => ctx.Source.Operator);

            Field(x => x.Value, nullable: true)
                .Name("value")
                .Description("Value to search for");
        }
    }
}