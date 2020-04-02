using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class ManagementGroupRates : ObjectGraphType<Models.Entities.ManagementGroupRates>
    {
        public ManagementGroupRates()
        {
            Field<ManagementGroupProvisionalFunding>("provisionalFunding",
                resolve: ctx => ctx.Source.ProvisionalFunding);
        }
    }
}
