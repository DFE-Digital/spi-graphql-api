using System;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class LineageAlternativeModel
    {
        public string Value { get; set; }
        public string AdapterName { get; set; }
        public DateTime? ReadDate { get; set; }
    }
    
    public class LineageAlternativeEntry : ObjectGraphType<LineageAlternativeModel>
    {
        public LineageAlternativeEntry()
        {
            Field(x => x.Value, nullable: true)
                .Name("value")
                .Description("value");
            
            Field(x => x.AdapterName, nullable: true)
                .Name("adapterName")
                .Description("adapterName");

            Field<DateTimeGraphType>(
                name: "readDate",
                description: "readDate",
                resolve: ctx => ctx.Source.ReadDate);
        }
    }
}