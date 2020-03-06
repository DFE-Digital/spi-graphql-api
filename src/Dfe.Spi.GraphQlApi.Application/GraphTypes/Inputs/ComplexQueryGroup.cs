using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes.Inputs
{
    public class ComplexQueryGroupModel
    {
        public ComplexQueryConditionModel[] Conditions { get; set; }
        public bool IsOr { get; set; }
    }
    
    public class ComplexQueryGroup : InputObjectGraphType<ComplexQueryGroupModel>
    {
        public ComplexQueryGroup()
        {
            Field<ListGraphType<ComplexQueryCondition>>(
                name: "conditions",
                description: "Conditions of query group",
                resolve: ctx => ctx.Source.Conditions);

            Field(x => x.IsOr, nullable: true)
                .Name("IsOr")
                .Description("Whether to treat conditions as or");
        }
    }
}