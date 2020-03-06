using Dfe.Spi.Common.Models;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs
{
    public class AggregationRequestModel
    {
        public string Name { get; set; }
        public DataFilter[] Conditions { get; set; }
    }
    
    public class AggregationRequest : InputObjectGraphType<AggregationRequestModel>
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