using Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class Census : ObjectGraphType<Models.Entities.Census>
    {
        public Census()
        {
            Field(x => x.Name)
                .Name("name")
                .Description("Name");
            
            Field<ListGraphType<Aggregation>>(
                name: "_aggregations",
                description: "Results of requested aggregations",
                resolve: ctx => ctx.Source._Aggregations,
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<AggregationRequest>>{ Name= "definitions" }));
        }
    }
}