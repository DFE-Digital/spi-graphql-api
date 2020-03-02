using System;
using Dfe.Spi.GraphQlApi.Application.Resolvers;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class LineageEntryModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string AdapterName { get; set; }
        public DateTime? ReadDate { get; set; }
        public LineageAlternativeModel[] Alternatives { get; set; }
    }
    
    public class LineageEntry : ObjectGraphType<LineageEntryModel>
    {
        public LineageEntry(ILineageAlternativeResolver lineageAlternativeResolver)
        {
            Field(x => x.Name, nullable: true)
                .Name("name")
                .Description("name");
            
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

            Field<ListGraphType<LineageAlternativeEntry>>(
                name: "alternatives",
                description: "alternatives",
                resolve: lineageAlternativeResolver.ResolveAsync);
        }
    }
}