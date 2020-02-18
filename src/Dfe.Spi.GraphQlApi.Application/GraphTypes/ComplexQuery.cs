using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class ComplexQueryConditionModel
    {
        public string Field { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
    }
    public class ComplexQueryGroupModel
    {
        public ComplexQueryConditionModel[] Conditions { get; set; }
        public bool IsAnd { get; set; }
    }
    public class ComplexQueryModel
    {
        public ComplexQueryGroupModel[] Groups { get; set; }
        public bool IsAnd { get; set; }
    }
    
    public class ComplexQueryCondition : InputObjectGraphType<ComplexQueryConditionModel>
    {
        public ComplexQueryCondition()
        {
            Field(x => x.Field)
                .Name("field")
                .Description("Name of the field to query on");
            
            Field(x => x.Operator)
                .Name("operator")
                .Description("Operator to use to compare");
            
            Field(x => x.Operator)
                .Name("value")
                .Description("Value to search for");
        }
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