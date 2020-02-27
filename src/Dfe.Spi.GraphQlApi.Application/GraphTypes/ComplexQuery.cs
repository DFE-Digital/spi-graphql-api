using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class ComplexQueryModel
    {
        public ComplexQueryGroupModel[] Groups { get; set; }
        public bool IsAnd { get; set; }
    }

    public class ComplexQuery : InputObjectGraphType<ComplexQueryModel>
    {
        public ComplexQuery()
        {
            Field<ListGraphType<ComplexQueryGroup>>(
                name: "groups",
                description: "groups of query",
                resolve: ctx => ctx.Source.Groups);

            Field(x => x.IsAnd)
                .Name("isAnd")
                .Description("Whether to treat groups as and")
                .DefaultValue(true);
        }
    }
}