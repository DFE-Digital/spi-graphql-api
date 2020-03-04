using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs
{
    public class AggregationRequest : InputObjectGraphType<Domain.Repository.AggregationRequest>
    {
        public AggregationRequest()
        {
            Field(x => x.Name)
                .Name("name")
                .Description("Name");
            
            Field<ListGraphType<AggregationRequestCondition>>(
                name: "conditions",
                description: "Conditions of query group",
                resolve: ctx => ctx.Source.Conditions);
        }
    }
}