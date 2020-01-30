using Dfe.Spi.Models;
using GraphQL.Types;

namespace Dfe.Spi.GraphQlApi.Application.GraphTypes
{
    public class LearningProviderType : ObjectGraphType<LearningProvider>
    {
        public LearningProviderType()
        {
            Field(x => x.Name)
                .Name("name")
                .Description("Name of the learning provider");

            Field(x => x.LegalName, nullable:true)
                .Name("legalName")
                .Description("Legal name of the learning provider");
            
            Field(x => x.Urn, nullable:true)
                .Name("urn")
                .Description("URN of the learning provider");
            
            Field(x => x.Ukprn, nullable:true)
                .Name("ukprn")
                .Description("UKPRN of the learning provider");
            
            Field(x => x.Postcode, nullable:true)
                .Name("postcode")
                .Description("Postcode of the learning provider");
            
            Field(x => x.OpenDate, nullable:true)
                .Name("openDate")
                .Description("Date the learning provider opened");
            
            Field(x => x.CloseDate, nullable:true)
                .Name("closeDate")
                .Description("Date the learning provider closed");

            Field<LearningProviderStatusEnum>(
                name: "Status",
                description: "Status of learning provider",
                resolve: ResolveStatus);

            Field<LearningProviderTypeEnum>(
                name: "Type",
                description: "Type of learning provider",
                resolve: ResolveType);

            Field<LearningProviderSubTypeEnum>(
                name: "SubType",
                description: "Sub-Type of learning provider",
                resolve: ResolveSubType);
        }

        private object ResolveStatus(ResolveFieldContext<LearningProvider> ctx)
        {
            return ctx.Source.Status;
        }
        private object ResolveType(ResolveFieldContext<LearningProvider> ctx)
        {
            return ctx.Source.Type;
        }
        private object ResolveSubType(ResolveFieldContext<LearningProvider> ctx)
        {
            return ctx.Source.SubType;
        }
    }
}