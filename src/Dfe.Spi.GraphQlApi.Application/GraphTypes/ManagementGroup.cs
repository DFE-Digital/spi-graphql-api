using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class ManagementGroup : ObjectGraphType<Models.Entities.ManagementGroup>
    {
        public ManagementGroup(
            ICensusResolver censusResolver,
            IManagementGroupRatesResolver managementGroupRatesResolver)
        {
            Field(x => x.Name, nullable: true)
                .Name("name")
                .Description("Name");
            
            Field(x => x.Code, nullable: true)
                .Name("code")
                .Description("Code");
            
            Field(x => x.Type, nullable: true)
                .Name("type")
                .Description("Type");
            
            Field(x => x.Identifier, nullable: true)
                .Name("identifier")
                .Description("Identifier");

            Field<Census>("census",
                resolve: censusResolver.ResolveAsync,
                arguments: new QueryArguments(new QueryArgument[]
                {
                    new QueryArgument<IntGraphType> {Name = "year"},
                    new QueryArgument<StringGraphType> {Name = "type"},
                }));

            Field<ManagementGroupRates>("rates",
                resolve: managementGroupRatesResolver.ResolveAsync,
                arguments: new QueryArguments(new QueryArgument[]
                {
                    new QueryArgument<IntGraphType> {Name = "year"},
                }));
        }
    }
}