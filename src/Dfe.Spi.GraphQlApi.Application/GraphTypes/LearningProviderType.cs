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
            
            
            
            Field(x => x.AcademyTrustCode, nullable:true)
                .Name("academyTrustCode")
                .Description("Academy trust code");
            
            Field(x => x.CharitiesCommissionNumber, nullable:true)
                .Name("charitiesCommissionNumber")
                .Description("Registered charity number");
            
            Field(x => x.CompaniesHouseNumber, nullable:true)
                .Name("companiesHouseNumber")
                .Description("Registered company number");
            
            Field(x => x.DfeNumber, nullable:true)
                .Name("dfeNumber")
                .Description("DfE Number");
            
            Field(x => x.LocalAuthority, nullable:true)
                .Name("localAuthority")
                .Description("Local authority");
            
            Field(x => x.EstablishmentNumber, nullable:true)
                .Name("establishmentNumber")
                .Description("Establishment number");
            
            Field(x => x.PreviousEstablishmentNumber, nullable:true)
                .Name("previousEstablishmentNumber")
                .Description("Previous establishment number");
            
            Field(x => x.Uprn, nullable:true)
                .Name("uprn")
                .Description("UPRN");
            
            

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