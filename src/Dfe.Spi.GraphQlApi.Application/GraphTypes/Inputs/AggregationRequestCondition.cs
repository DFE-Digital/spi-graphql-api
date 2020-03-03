using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs
{
    public class AggregationRequestConditionModel
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
    public class AggregationRequestCondition : InputObjectGraphType<ComplexQueryConditionModel>
    {
        public AggregationRequestCondition()
        {
            Field(x => x.Field)
                .Name("field")
                .Description("Name of the field to query on");

            Field(x => x.Operator, nullable: true)
                .Name("operator")
                .Description("Operator to use to compare");

            Field(x => x.Value, nullable: true)
                .Name("value")
                .Description("Value to search for");
        }
    }
}