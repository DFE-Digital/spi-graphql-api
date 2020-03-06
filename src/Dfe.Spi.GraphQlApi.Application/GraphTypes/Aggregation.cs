using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class Aggregation : ObjectGraphType<Models.Aggregation>
    {
        public Aggregation()
        {
            Field(x => x.Name)
                .Name("name")
                .Description("Name");
            
            Field(x => x.Value, nullable: true)
                .Name("value")
                .Description("Value");
        }
    }
}