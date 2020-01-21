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

            Field<LearningProviderStatusEnum>(
                name: "Status",
                description: "Status of learning provider",
                resolve: ResolveStatus);
        }

        private object ResolveStatus(ResolveFieldContext<LearningProvider> ctx)
        {
            return "One";
        }
    }

    public class LearningProviderStatusEnum : EnumerationGraphType
    {
        public LearningProviderStatusEnum()
        {
            Name = "Status";
            Description = "Status of learning provider";
            AddValue("One", "First value", "One");
            AddValue("Two", "Second value", "Two");
            AddValue("Three", "Third value", "Three");
        }
    }
}