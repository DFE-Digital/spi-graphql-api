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
            
            Field(x => x.CompaniesHouseNumber, nullable: true)
                .Name("companiesHouseNumber")
                .Description("Companies House Number");
            
            Field(x => x.Ukprn, nullable: true)
                .Name("ukprn")
                .Description("Ukprn");
            
            Field(x => x.AddressLine1, nullable: true)
                .Name("addressLine1")
                .Description("Address Line 1");
            
            Field(x => x.AddressLine2, nullable: true)
                .Name("addressLine2")
                .Description("Address Line 2");
            
            Field(x => x.AddressLine3, nullable: true)
                .Name("addressLine3")
                .Description("Address Line 3");
            
            Field(x => x.Town, nullable: true)
                .Name("town")
                .Description("Town");
            
            Field(x => x.County, nullable: true)
                .Name("county")
                .Description("County");
            
            Field(x => x.Postcode, nullable: true)
                .Name("postcode")
                .Description("Postcode");

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