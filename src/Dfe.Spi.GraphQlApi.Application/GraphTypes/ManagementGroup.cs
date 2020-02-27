using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class ManagementGroup : ObjectGraphType<Models.Entities.ManagementGroup>
    {
        public ManagementGroup()
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
        }
    }
}