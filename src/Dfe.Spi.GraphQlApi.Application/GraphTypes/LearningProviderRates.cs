using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class LearningProviderRates : ObjectGraphType<Models.Entities.LearningProviderRates>
    {
        public LearningProviderRates()
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