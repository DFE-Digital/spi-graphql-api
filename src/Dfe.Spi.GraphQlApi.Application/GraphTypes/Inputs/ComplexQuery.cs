using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs
{
    public class ComplexQueryModel
    {
        public ComplexQueryGroupModel[] Groups { get; set; }
        public bool IsOr { get; set; }
    }

    public class ComplexQuery : InputObjectGraphType<ComplexQueryModel>
    {
        public ComplexQuery()
        {
            Field<ListGraphType<ComplexQueryGroup>>(
                name: "groups",
                description: "groups of query",
                resolve: ctx => ctx.Source.Groups);

            Field(x => x.IsOr, nullable: true)
                .Name("isOr")
                .Description("Whether to treat groups as or");
        }
    }
}