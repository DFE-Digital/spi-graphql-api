using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class ComplexQueryGroupModel
    {
        public ComplexQueryConditionModel[] Conditions { get; set; }
        public bool IsAnd { get; set; }
    }
    
    public class ComplexQueryGroup : InputObjectGraphType<ComplexQueryGroupModel>
    {
        public ComplexQueryGroup()
        {
            Field<ListGraphType<ComplexQueryCondition>>(
                name: "conditions",
                description: "Conditions of query group",
                resolve: ctx => ctx.Source.Conditions);

            Field(x => x.IsAnd)
                .Name("isAnd")
                .Description("Whether to treat conditions as and")
                .DefaultValue(true);
        }
    }
}