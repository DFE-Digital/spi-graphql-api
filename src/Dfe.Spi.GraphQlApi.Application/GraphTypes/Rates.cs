using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class Rates : ObjectGraphType<Models.Entities.Rates>
    {
        public Rates()
        {
            Field<BaselineFunding>("baselineFunding",
                resolve: ctx => ctx.Source.BaselineFunding);
            
            Field<IllustrativeFunding>("illustrativeFunding",
                resolve: ctx => ctx.Source.IllustrativeFunding);
            
            Field<NotionalFunding>("notionalFunding",
                resolve: ctx => ctx.Source.NotionalFunding);
        }
    }
}